// Modelo principal retornado pela API
export type Contact = {
  id: string
  name: string
  email: string
  phone: string
  createdAt: string
  updatedAt: string | null
}

// Payloads enviados para API
export type CreateContactRequest = {
  name: string
  email: string
  phone: string
}

export type UpdateContactRequest = {
  name: string
  email: string
  phone: string
}