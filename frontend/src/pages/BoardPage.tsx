import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import {
  createCard,
  deleteCard,
  listCards,
  renameCard,
  type CardResponse,
} from '../lib/workspacesApi'

function BoardPage() {
  const { workspaceId, boardId } = useParams<{ workspaceId: string; boardId: string }>()
  const [cards, setCards] = useState<CardResponse[]>([])
  const [title, setTitle] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    if (!boardId) {
      return
    }

    listCards(boardId)
      .then(setCards)
      .catch((err) => setError(err instanceof Error ? err.message : 'Falha ao carregar cards.'))
      .finally(() => setIsLoading(false))
  }, [boardId])

  async function handleCreateCard(event: React.FormEvent) {
    event.preventDefault()
    if (!boardId) {
      return
    }
    setError(null)
    setIsSubmitting(true)

    try {
      const card = await createCard(boardId, title)
      setCards((current) => [...current, card])
      setTitle('')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao criar card.')
    } finally {
      setIsSubmitting(false)
    }
  }

  async function handleRenameCard(card: CardResponse) {
    const newTitle = window.prompt('Novo título do card', card.title)
    if (!newTitle || newTitle === card.title) {
      return
    }
    setError(null)

    try {
      const updated = await renameCard(card.id, newTitle)
      setCards((current) => current.map((c) => (c.id === updated.id ? updated : c)))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao renomear card.')
    }
  }

  async function handleDeleteCard(cardId: string) {
    setError(null)

    try {
      await deleteCard(cardId)
      setCards((current) => current.filter((card) => card.id !== cardId))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao excluir card.')
    }
  }

  if (isLoading) {
    return (
      <div className="page">
        <p className="muted">Carregando...</p>
      </div>
    )
  }

  return (
    <div className="page">
      <div className="page-header">
        <h1>Cards</h1>
        <Link to={`/workspaces/${workspaceId}`}>Voltar</Link>
      </div>

      <form className="inline-form" onSubmit={handleCreateCard}>
        <input placeholder="Título do novo card" value={title} onChange={(e) => setTitle(e.target.value)} required />
        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Criando...' : 'Criar'}
        </button>
      </form>

      {error && <div className="chat-error">{error}</div>}

      {cards.length === 0 ? (
        <p className="muted">Nenhum card ainda. Crie o primeiro acima.</p>
      ) : (
        <ul className="card-list">
          {cards.map((card) => (
            <li key={card.id}>
              <span className="card-name">{card.title}</span>
              <div className="card-actions">
                <button type="button" onClick={() => handleRenameCard(card)}>
                  Renomear
                </button>
                <button type="button" onClick={() => handleDeleteCard(card.id)}>
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

export default BoardPage
