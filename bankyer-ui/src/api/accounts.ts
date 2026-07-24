import { createHttpClient } from './http'

export type Account = {
  id: string
  status: string
  balance: number
  currency: {
    code: string
    name: string
  }
}

export type CreateAccountRequest = {
  initialAmount: number
  currency: string
}

const http = createHttpClient(import.meta.env.VITE_API_URL ?? '')

// Keep endpoint details here, so components use named, typed operations.
export const accountsApi = {
  list: () => http.get<Account[]>('/api/accounts'),
  create: (request: CreateAccountRequest) => http.post('/api/accounts', request),
}
