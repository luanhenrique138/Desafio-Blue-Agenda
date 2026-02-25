<script setup lang="ts">
import type { Contact } from "@/types/contacts"

const props = defineProps<{
  contacts: Contact[]
  loading?: boolean
  error?: string | null
}>()

const emit = defineEmits<{
  (e: "edit", contact: Contact): void
  (e: "delete", contact: Contact): void
}>()
</script>

<template>
  <v-alert
    v-if="props.error"
    type="error"
    variant="tonal"
    class="mb-3"
    :text="props.error"
  />
  <v-table>
    <thead>
      <tr>
        <th>Nome</th>
        <th>Email</th>
        <th>Telefone</th>
        <th class="text-right">Ações</th>
      </tr>
    </thead>

    <tbody>
      
      <tr v-if="props.loading">
        <td colspan="4">
          <v-skeleton-loader type="table-row@6" />
        </td>
      </tr>

      <tr v-else-if="!props.contacts.length">
        <td colspan="4" class="text-center text-medium-emphasis py-6">
          Nenhum contato cadastrado
        </td>
      </tr>

      <tr v-else v-for="c in contacts" :key="c.id">
        <td>{{ c.name }}</td>
        <td>{{ c.email }}</td>
        <td>{{ c.phone }}</td>

        <td class="text-right">
          <v-tooltip text="Editar" location="top">
            <template #activator="{ props }">
              <v-btn
                v-bind="props"
                icon="mdi-pencil"
                variant="text"
                @click="emit('edit', c)"
              />
            </template>
          </v-tooltip>

          <v-tooltip text="Excluir" location="top">
            <template #activator="{ props }">
              <v-btn
                v-bind="props"
                icon="mdi-delete"
                variant="text"
                @click="emit('delete', c)"
              />
            </template>
          </v-tooltip>
        </td>
      </tr>
    </tbody>
  </v-table>
</template>