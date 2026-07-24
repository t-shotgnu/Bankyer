import { createHttpClient } from './http'

export type CurrentUser = {
  id: string
  userName: string | null
  email: string | null
}

type Credentials = {
  email: string
  password: string
}

const http = createHttpClient(import.meta.env.VITE_API_URL ?? '')

export const authApi = {
  register: (credentials: Credentials) => http.post('/api/auth/register', credentials),
  login: (credentials: Credentials) => http.post('/api/auth/login', credentials),
  me: () => http.get<CurrentUser>('/api/auth/me'),
}
