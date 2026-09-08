import { request } from './api'

export interface WorkspaceResponse {
  id: string
  name: string
  ownerId: string
  createdAt: string
}

export interface BoardResponse {
  id: string
  name: string
  workspaceId: string
  createdAt: string
}

export function listWorkspaces(): Promise<WorkspaceResponse[]> {
  return request<WorkspaceResponse[]>('/api/workspaces', { method: 'GET' })
}

export function getWorkspace(workspaceId: string): Promise<WorkspaceResponse> {
  return request<WorkspaceResponse>(`/api/workspaces/${workspaceId}`, { method: 'GET' })
}

export function createWorkspace(name: string): Promise<WorkspaceResponse> {
  return request<WorkspaceResponse>('/api/workspaces', { method: 'POST', body: { name } })
}

export function renameWorkspace(workspaceId: string, name: string): Promise<WorkspaceResponse> {
  return request<WorkspaceResponse>(`/api/workspaces/${workspaceId}`, { method: 'PUT', body: { name } })
}

export function deleteWorkspace(workspaceId: string): Promise<void> {
  return request<void>(`/api/workspaces/${workspaceId}`, { method: 'DELETE' })
}

export function listBoards(workspaceId: string): Promise<BoardResponse[]> {
  return request<BoardResponse[]>(`/api/workspaces/${workspaceId}/boards`, { method: 'GET' })
}

export function createBoard(workspaceId: string, name: string): Promise<BoardResponse> {
  return request<BoardResponse>(`/api/workspaces/${workspaceId}/boards`, { method: 'POST', body: { name } })
}

export function renameBoard(boardId: string, name: string): Promise<BoardResponse> {
  return request<BoardResponse>(`/api/boards/${boardId}`, { method: 'PUT', body: { name } })
}

export function deleteBoard(boardId: string): Promise<void> {
  return request<void>(`/api/boards/${boardId}`, { method: 'DELETE' })
}

export interface WorkspaceMemberResponse {
  userId: string
  username: string
  email: string
  joinedAt: string
}

export function listWorkspaceMembers(workspaceId: string): Promise<WorkspaceMemberResponse[]> {
  return request<WorkspaceMemberResponse[]>(`/api/workspaces/${workspaceId}/members`, { method: 'GET' })
}

export function inviteWorkspaceMember(workspaceId: string, email: string): Promise<WorkspaceMemberResponse> {
  return request<WorkspaceMemberResponse>(`/api/workspaces/${workspaceId}/members`, { method: 'POST', body: { email } })
}

export function removeWorkspaceMember(workspaceId: string, userId: string): Promise<void> {
  return request<void>(`/api/workspaces/${workspaceId}/members/${userId}`, { method: 'DELETE' })
}

export interface CardResponse {
  id: string
  title: string
  boardId: string
  createdAt: string
}

export function listCards(boardId: string): Promise<CardResponse[]> {
  return request<CardResponse[]>(`/api/boards/${boardId}/cards`, { method: 'GET' })
}

export function createCard(boardId: string, title: string): Promise<CardResponse> {
  return request<CardResponse>(`/api/boards/${boardId}/cards`, { method: 'POST', body: { title } })
}

export function renameCard(cardId: string, title: string): Promise<CardResponse> {
  return request<CardResponse>(`/api/cards/${cardId}`, { method: 'PUT', body: { title } })
}

export function deleteCard(cardId: string): Promise<void> {
  return request<void>(`/api/cards/${cardId}`, { method: 'DELETE' })
}
