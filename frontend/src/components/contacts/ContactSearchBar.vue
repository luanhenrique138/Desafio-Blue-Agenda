<script setup lang="ts">
import { computed } from "vue"

type Props = {
  modelValue: string
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
})

const emit = defineEmits<{
  (e: "update:modelValue", value: string): void
  (e: "clear"): void
}>()

const value = computed({
  get: () => props.modelValue,
  set: (v: string) => emit("update:modelValue", v),
})

function onClear() {
  emit("update:modelValue", "")
  emit("clear")
}
</script>

<template>
  <v-text-field
    v-model="value"
    label="Buscar contato"
    placeholder="Nome, email ou telefone..."
    prepend-inner-icon="mdi-magnify"
    variant="outlined"
    density="compact"
    size="larger"
    hide-details
    single-line
    :loading="loading"
    clearable
    @click:clear="onClear"
  />
</template>