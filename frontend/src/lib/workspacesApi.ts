import { request } from './api'

export interface WorkspaceResponse {
  id: string
  name: string
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
