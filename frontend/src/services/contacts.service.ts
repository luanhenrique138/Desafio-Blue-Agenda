import type { Contact, CreateContactRequest, UpdateContactRequest } from '@/types/contacts';
import { http } from './http';
import type { PagedResult } from '@/types/paged';

export const ContactsService = {
    async getAllContacts(params?: {search?: string, page?: number, pageSize?:number}): Promise<PagedResult<Contact>> {
        const { data } = await http.get<PagedResult<Contact>>('/Contacts', {
            params: {
                search: params?.search ?? undefined,
                page: params?.page ?? 1,
                pageSize: params?.pageSize ?? 10,
                // search ? { search } : {}
            }
        })

        return data;
    },

    async getContactById(id: string): Promise<Contact> {
        const { data } = await http.get<Contact>(`/Contacts/${id}`)
        return data;
    },

    async createContact(payload: CreateContactRequest): Promise<Contact> {
        const { data } = await http.post<Contact>('/Contacts', payload);
        return data;
    },

    async updateContact(id: string, payload: UpdateContactRequest): Promise<Contact> {
        const { data } = await http.put<Contact>(`/Contacts/${id}`, payload);
        return data
    },

    async deleteContact(id: string): Promise<void> {
        await http.delete(`/Contacts/${id}`);
    }

    
}