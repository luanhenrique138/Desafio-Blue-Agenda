<script setup lang="ts">
import type { Contact } from "@/types/contacts"

const props = defineProps<{
  modelValue: boolean
  contact: Contact | null
  loading?: boolean
  error?: string | null
}>()

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void
  (e: "confirm"): void
  (e: "cancel"): void
}>()

function close() {
  emit("update:modelValue", false)
  emit("cancel")
}
</script>

<template>
  <v-dialog :model-value="modelValue" max-width="480" @update:modelValue="emit('update:modelValue', $event)">
    <v-card>
      <v-card-title>Confirmar exclusão</v-card-title>

      <v-card-text>
        Tem certeza que deseja excluir o contato
        <strong>{{ contact?.name }}</strong>?
        <div class="text-caption text-medium-emphasis mt-2">
          Essa ação não pode ser desfeita.
        </div>

        <v-alert
          v-if="error"
          type="error"
          variant="tonal"
          class="mt-3"
          :text="error"
        />
      </v-card-text>

      <v-card-actions>
        <v-spacer />
        <v-btn variant="text" @click="close" :disabled="loading">
          Cancelar
        </v-btn>
        <v-btn color="red" @click="emit('confirm')" :loading="loading">
          Excluir
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>