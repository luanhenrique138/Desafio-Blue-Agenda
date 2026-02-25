<script setup lang="ts">
import { computed, reactive, watch } from "vue"
import type { Contact, CreateContactRequest } from "@/types/contacts"
import {VMaskInput} from 'vuetify/labs/VMaskInput';

const props = defineProps<{
  modelValue: boolean
  mode: "create" | "edit"
  initialValues?: Partial<Contact> | null
  loading?: boolean
  error?: string | null
}>()

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void
  (e: "save", payload: CreateContactRequest): void
  (e: "cancel"): void
}>()

const title = computed(() => (props.mode === "edit" ? "Editar contato" : "Novo contato"))

const form = reactive<CreateContactRequest>({
  name: "",
  email: "",
  phone: "",
})

const isOpen = computed({
  get: () => props.modelValue,
  set: (v: boolean) => emit("update:modelValue", v),
})

// Sempre que abrir ou mudar initialValues, preencher formulário
watch(
  () => [props.modelValue, props.initialValues] as const,
  ([open]) => {
    if (!open) return
    form.name = props.initialValues?.name ?? ""
    form.email = props.initialValues?.email ?? ""
    form.phone = props.initialValues?.phone ?? ""
  }
)

function close() {
  isOpen.value = false
  emit("cancel")
}

function onlyDigits(v: string) {
  return (v ?? "").replace(/\D/g, "")
}

function submit() {
  emit("save", {
    name: form.name.trim(),
    email: form.email.trim(),
    phone: onlyDigits(form.phone),
  })
}

// Regras simples (pode evoluir depois)
const rules = {
  required: (v: string) => (!!v && v.trim().length > 0) || "Campo obrigatório",
  email: (v: string) =>
    /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v) || "Email inválido",
  minPhone: (v: string) => {
    const len = (v ?? "").replace(/\D/g, "").length;
    if (len === 0) return true;
    return (len === 10 || len === 11) || "Telefone Invalido";
  },
}

const phoneMask = computed(() => {
  const digits = (form.phone ?? "").replace(/\D/g, "");
  
  if (digits.length > 10) {
    return "(##) #####-####";
  }
  
  return "(##) ####-####" + (digits.length === 10 ? "#" : "");
});


</script>

<template>
  <v-dialog v-model="isOpen" max-width="520">
    <v-card class="pa-5">
      <v-card-title>{{ title }}</v-card-title>

      <v-card-text>
        <v-form>
          <v-text-field
            v-model="form.name"
            label="Nome"
            variant="underlined"
            density="comfortable"
            :rules="[rules.required]"
          />

          <v-text-field
            v-model="form.email"
            label="Email"
            type="email"
            variant="underlined"
            density="comfortable"
            :rules="[rules.required, rules.email]"
          />

          <v-mask-input
            v-model="form.phone"
            :mask="phoneMask"
            
            
            label="Telefone"
            variant="underlined"
            density="comfortable"
            :rules="[rules.required, rules.minPhone]"
          />

          <v-alert
            v-if="error"
            type="error"
            variant="tonal"
            class="mt-3"
            :text="error"
          />
        </v-form>
      </v-card-text>

      <v-card-actions>
        <v-spacer />
        <v-btn variant="text" @click="close" :disabled="loading">
          Cancelar
        </v-btn>
        <v-btn color="primary" @click="submit" :loading="loading">
          Salvar
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>