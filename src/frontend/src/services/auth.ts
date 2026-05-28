const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5206'

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  expiracao: string
}

export async function login(email: string, senha: string): Promise<LoginResponse> {
  const res = await fetch(`${API_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, senha }),
  })

  if (!res.ok) {
    const erro = await res.json().catch(() => null)
    throw new Error(erro?.message ?? 'Credenciais inválidas')
  }

  return res.json()
}
