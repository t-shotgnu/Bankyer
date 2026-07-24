type RequestOptions = Omit<RequestInit, 'body' | 'headers'> & {
  body?: unknown
  headers?: HeadersInit
}

export class ApiError extends Error {
  public readonly status: number

  constructor(
    status: number,
    message: string,
  ) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

type ApiResponse<T> = {
  success: boolean
  data: T | null
  error: {
    code: string
    message: string
  } | null
}

export function createHttpClient(baseUrl = '') {
  async function request<TResponse>(path: string, options: RequestOptions = {}): Promise<TResponse> {
    const { body, headers, ...init } = options
    const response = await fetch(`${baseUrl}${path}`, {
      credentials: 'include',
      ...init,
      headers: body === undefined
        ? headers
        : { 'Content-Type': 'application/json', ...headers },
      body: body === undefined ? undefined : JSON.stringify(body),
    })

    const payload = await response.json() as ApiResponse<TResponse>
    if (!response.ok || !payload.success) {
      throw new ApiError(response.status, payload.error?.message ?? `Request failed with status ${response.status}.`)
    }

    return payload.data as TResponse
  }

  return {
    get: <TResponse>(path: string, options?: Omit<RequestOptions, 'body'>) =>
      request<TResponse>(path, options),
    post: <TResponse = void>(path: string, body?: unknown, options?: Omit<RequestOptions, 'body'>) =>
      request<TResponse>(path, { ...options, method: 'POST', body }),
    put: <TResponse = void>(path: string, body?: unknown, options?: Omit<RequestOptions, 'body'>) =>
      request<TResponse>(path, { ...options, method: 'PUT', body }),
    delete: <TResponse = void>(path: string, options?: Omit<RequestOptions, 'body'>) =>
      request<TResponse>(path, { ...options, method: 'DELETE' }),
  }
}
