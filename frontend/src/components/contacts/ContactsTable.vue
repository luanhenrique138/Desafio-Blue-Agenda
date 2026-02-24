<script setup lang="ts">
import type { Contact } from "@/types/contacts"

defineProps<{
  contacts: Contact[]
}>()

const emit = defineEmits<{
  (e: "edit", contact: Contact): void
  (e: "delete", contact: Contact): void
}>()
</script>

<template>
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
      <tr v-for="c in contacts" :key="c.id">
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