import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { createWorkspace, deleteWorkspace, listWorkspaces, renameWorkspace, type WorkspaceResponse } from '../lib/workspacesApi'

function HomePage() {
  const { user, logout } = useAuth()
  const [workspaces, setWorkspaces] = useState<WorkspaceResponse[]>([])
  const [name, setName] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    listWorkspaces()
      .then(setWorkspaces)
      .catch((err) => setError(err instanceof Error ? err.message : 'Falha ao carregar workspaces.'))
      .finally(() => setIsLoading(false))
  }, [])

  async function handleRename(workspace: WorkspaceResponse) {
    const newName = window.prompt('Novo nome do workspace', workspace.name)
    if (!newName || newName === workspace.name) {
      return
    }
    setError(null)

    try {
      const updated = await renameWorkspace(workspace.id, newName)
      setWorkspaces((current) => current.map((w) => (w.id === updated.id ? updated : w)))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao renomear workspace.')
    }
  }

  async function handleDelete(workspaceId: string) {
    setError(null)

    try {
      await deleteWorkspace(workspaceId)
      setWorkspaces((current) => current.filter((w) => w.id !== workspaceId))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao excluir workspace.')
    }
  }

  async function handleCreate(event: React.FormEvent) {
    event.preventDefault()
    setError(null)
    setIsSubmitting(true)

    try {
      const workspace = await createWorkspace(name)
      setWorkspaces((current) => [...current, workspace])
      setName('')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao criar workspace.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="page">
      <div className="page-header">
        <h1>Workspaces de {user?.username}</h1>
        <button type="button" onClick={logout}>
          Sair
        </button>
      </div>

      <form className="inline-form" onSubmit={handleCreate}>
        <input
          placeholder="Nome do novo workspace"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
        />
        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Criando...' : 'Criar'}
        </button>
      </form>

      {error && <div className="chat-error">{error}</div>}

      {isLoading ? (
        <p className="muted">Carregando...</p>
      ) : workspaces.length === 0 ? (
        <p className="muted">Você ainda não tem nenhum workspace. Crie o primeiro acima.</p>
      ) : (
        <ul className="card-list">
          {workspaces.map((workspace) => (
            <li key={workspace.id}>
              <Link to={`/workspaces/${workspace.id}`}>{workspace.name}</Link>
              <div className="card-actions">
                <button type="button" onClick={() => handleRename(workspace)}>
                  Renomear
                </button>
                <button type="button" onClick={() => handleDelete(workspace.id)}>
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

export default HomePage
