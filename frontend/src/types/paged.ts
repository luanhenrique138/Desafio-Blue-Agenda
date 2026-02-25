import type { Contact } from "./contacts"

export type PagedResult<T> = {
  items: T[]
  page: number
  pageSize: number
  totalItems: number
  totalpages: number
}