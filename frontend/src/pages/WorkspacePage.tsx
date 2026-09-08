import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import {
  createBoard,
  deleteBoard,
  deleteWorkspace,
  getWorkspace,
  inviteWorkspaceMember,
  listBoards,
  listWorkspaceMembers,
  removeWorkspaceMember,
  renameBoard,
  type BoardResponse,
  type WorkspaceMemberResponse,
  type WorkspaceResponse,
} from '../lib/workspacesApi'

function WorkspacePage() {
  const { workspaceId } = useParams<{ workspaceId: string }>()
  const { user } = useAuth()
  const navigate = useNavigate()
  const [workspace, setWorkspace] = useState<WorkspaceResponse | null>(null)
  const [boards, setBoards] = useState<BoardResponse[]>([])
  const [members, setMembers] = useState<WorkspaceMemberResponse[]>([])
  const [name, setName] = useState('')
  const [inviteEmail, setInviteEmail] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [isInviting, setIsInviting] = useState(false)

  const isOwner = workspace?.ownerId === user?.userId

  useEffect(() => {
    if (!workspaceId) {
      return
    }

    Promise.all([getWorkspace(workspaceId), listBoards(workspaceId), listWorkspaceMembers(workspaceId)])
      .then(([workspaceResult, boardsResult, membersResult]) => {
        setWorkspace(workspaceResult)
        setBoards(boardsResult)
        setMembers(membersResult)
      })
      .catch((err) => setError(err instanceof Error ? err.message : 'Falha ao carregar workspace.'))
      .finally(() => setIsLoading(false))
  }, [workspaceId])

  async function handleCreateBoard(event: React.FormEvent) {
    event.preventDefault()
    if (!workspaceId) {
      return
    }
    setError(null)
    setIsSubmitting(true)

    try {
      const board = await createBoard(workspaceId, name)
      setBoards((current) => [...current, board])
      setName('')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao criar board.')
    } finally {
      setIsSubmitting(false)
    }
  }

  async function handleDeleteBoard(boardId: string) {
    setError(null)

    try {
      await deleteBoard(boardId)
      setBoards((current) => current.filter((board) => board.id !== boardId))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao excluir board.')
    }
  }

  async function handleRenameBoard(board: BoardResponse) {
    const newName = window.prompt('Novo nome do board', board.name)
    if (!newName || newName === board.name) {
      return
    }
    setError(null)

    try {
      const updated = await renameBoard(board.id, newName)
      setBoards((current) => current.map((b) => (b.id === updated.id ? updated : b)))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao renomear board.')
    }
  }

  async function handleDeleteWorkspace() {
    if (!workspaceId) {
      return
    }
    setError(null)

    try {
      await deleteWorkspace(workspaceId)
      navigate('/')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao excluir workspace.')
    }
  }

  async function handleInviteMember(event: React.FormEvent) {
    event.preventDefault()
    if (!workspaceId) {
      return
    }
    setError(null)
    setIsInviting(true)

    try {
      const member = await inviteWorkspaceMember(workspaceId, inviteEmail)
      setMembers((current) => [...current, member])
      setInviteEmail('')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao convidar membro.')
    } finally {
      setIsInviting(false)
    }
  }

  async function handleRemoveMember(userId: string) {
    if (!workspaceId) {
      return
    }
    setError(null)

    try {
      await removeWorkspaceMember(workspaceId, userId)
      setMembers((current) => current.filter((m) => m.userId !== userId))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao remover membro.')
    }
  }

  if (isLoading) {
    return (
      <div className="page">
        <p className="muted">Carregando...</p>
      </div>
    )
  }

  if (!workspace) {
    return (
      <div className="page">
        <p className="chat-error">{error ?? 'Workspace não encontrado.'}</p>
        <Link to="/">Voltar</Link>
      </div>
    )
  }

  return (
    <div className="page">
      <div className="page-header">
        <h1>{workspace.name}</h1>
        <div className="card-actions">
          <Link to="/">Voltar</Link>
          {isOwner && (
            <button type="button" onClick={handleDeleteWorkspace}>
              Excluir workspace
            </button>
          )}
        </div>
      </div>

      <form className="inline-form" onSubmit={handleCreateBoard}>
        <input placeholder="Nome do novo board" value={name} onChange={(e) => setName(e.target.value)} required />
        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Criando...' : 'Criar'}
        </button>
      </form>

      {error && <div className="chat-error">{error}</div>}

      {boards.length === 0 ? (
        <p className="muted">Nenhum board ainda. Crie o primeiro acima.</p>
      ) : (
        <ul className="card-list">
          {boards.map((board) => (
            <li key={board.id}>
              <Link to={`/workspaces/${workspaceId}/boards/${board.id}`}>{board.name}</Link>
              <div className="card-actions">
                <button type="button" onClick={() => handleRenameBoard(board)}>
                  Renomear
                </button>
                <button type="button" onClick={() => handleDeleteBoard(board.id)}>
                  Excluir
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}

      <h2>Membros</h2>

      {isOwner && (
        <form className="inline-form" onSubmit={handleInviteMember}>
          <input
            type="email"
            placeholder="E-mail de um usuário cadastrado"
            value={inviteEmail}
            onChange={(e) => setInviteEmail(e.target.value)}
            required
          />
          <button type="submit" disabled={isInviting}>
            {isInviting ? 'Convidando...' : 'Convidar'}
          </button>
        </form>
      )}

      <ul className="card-list">
        {members.map((member) => (
          <li key={member.userId}>
            <span className="card-name">
              {member.username} {member.userId === workspace.ownerId && '(dono)'}
            </span>
            {isOwner && member.userId !== workspace.ownerId && (
              <div className="card-actions">
                <button type="button" onClick={() => handleRemoveMember(member.userId)}>
                  Remover
                </button>
              </div>
            )}
          </li>
        ))}
      </ul>
    </div>
  )
}

export default WorkspacePage
