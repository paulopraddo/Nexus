import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
  createBoard,
  deleteBoard,
  deleteWorkspace,
  getWorkspace,
  listBoards,
  renameBoard,
  type BoardResponse,
  type WorkspaceResponse,
} from '../lib/workspacesApi'

function WorkspacePage() {
  const { workspaceId } = useParams<{ workspaceId: string }>()
  const navigate = useNavigate()
  const [workspace, setWorkspace] = useState<WorkspaceResponse | null>(null)
  const [boards, setBoards] = useState<BoardResponse[]>([])
  const [name, setName] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    if (!workspaceId) {
      return
    }

    Promise.all([getWorkspace(workspaceId), listBoards(workspaceId)])
      .then(([workspaceResult, boardsResult]) => {
        setWorkspace(workspaceResult)
        setBoards(boardsResult)
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
          <button type="button" onClick={handleDeleteWorkspace}>
            Excluir workspace
          </button>
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
    </div>
  )
}

export default WorkspacePage
