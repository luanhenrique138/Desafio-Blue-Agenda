<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue"
import { ContactsService } from "@/services/contacts.service"
import type { Contact, CreateContactRequest } from "@/types/contacts"

import ContactsTable from "@/components/contacts/ContactsTable.vue"
import ConfirmDeleteDialog from "@/components/contacts/ConfirmDeleteDialog.vue"
import ContactFormDialog from "@/components/contacts/ContactFormDialog.vue"
import ContactSearchBar from "@/components/contacts/ContactSearchBar.vue"
import type { PagedResult } from "@/types/paged"

const paged = ref<PagedResult<Contact> | null>(null)

const page = ref(1)
const pageSize = ref(10)
const search = ref("")

const loadingContacts = ref(false)
const error = ref<string | null>(null)

const contacts = computed(() => paged.value?.items ?? [])
const totalPages = computed(() => paged.value?.totalpages ?? 1)
const totalItems = computed(() => paged.value?.totalItems ?? 0)

const deleteDialog = ref(false)
const formDialog = ref(false)
const formMode = ref<"create" | "edit">("create")
const formInitialValues = ref<Contact | null>(null)
const saving = ref(false)
const formError = ref<string | null>(null)
const deleting = ref(false)
const selectContact = ref<Contact | null>(null)

async function fetchContacts() {
  try {
    loadingContacts.value = true
    error.value = null

    paged.value = await ContactsService.getAllContacts({
      search: search.value.trim() || undefined,
      page: page.value,
      pageSize: pageSize.value,
    })
  } catch {
    paged.value = { items: [], page: page.value, pageSize: pageSize.value, totalItems: 0, totalpages: 1 }
    error.value = "Falha ao carregar contatos."
  } finally {
    loadingContacts.value = false
  }
}

// debounce da busca + reset para página 1
let t: number | undefined
watch(search, () => {
  window.clearTimeout(t)
  t = window.setTimeout(() => {
    page.value = 1
    fetchContacts()
  }, 350)
})

watch([page, pageSize], () => {
  fetchContacts()
})

onMounted(fetchContacts)

// delete
function openDeleteDialog(contact: Contact) {
  selectContact.value = contact
  deleteDialog.value = true
}
function closeDeleteDialog() {
  deleteDialog.value = false
  selectContact.value = null
}

async function confirmDelete() {
  if (!selectContact.value) return

  try {
    deleting.value = true
    await ContactsService.deleteContact(selectContact.value.id)

    if (contacts.value.length === 1 && page.value > 1) page.value -= 1

    await fetchContacts()
    closeDeleteDialog()
  } finally {
    deleting.value = false
  }
}

function openCreateDialog() {
  formMode.value = "create"
  formInitialValues.value = null
  formError.value = null
  formDialog.value = true
}
function openEditDialog(contact: Contact) {
  formMode.value = "edit"
  formInitialValues.value = contact
  formError.value = null
  formDialog.value = true
}

async function handleSave(payload: CreateContactRequest) {
  try {
    saving.value = true
    formError.value = null

    if (formMode.value === "create") {
      await ContactsService.createContact(payload)
      page.value = 1 // opcional: sempre volta pra primeira página
    } else {
      if (!formInitialValues.value) return
      await ContactsService.updateContact(formInitialValues.value.id, payload)
    }

    formDialog.value = false
    await fetchContacts()
  } catch {
    formError.value = "Falha ao salvar contato."
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <v-container>
    <v-card>
      <v-card-title class="d-flex align-center justify-space-between">
        <span>Contatos</span>

        <div class="d-flex align-center ga-3">
          <ContactSearchBar v-model="search" class="flex-grow-1 mr-3" />

          <v-btn color="primary" prepend-icon="mdi-plus" @click="openCreateDialog">
            Adicionar
          </v-btn>
        </div>
      </v-card-title>

      <v-card-text>
        <ContactsTable
          :contacts="contacts"
          :error="error"
          :loading="loadingContacts"
          @edit="openEditDialog"
          @delete="openDeleteDialog"
        />

        <div class="d-flex align-center justify-space-between mt-4">
          <div class="text-caption text-medium-emphasis">
            Total: {{ totalItems }}
          </div>

          <div class="d-flex align-center ga-3">
            <v-select
              v-model="pageSize"
              :items="[5, 10, 20, 50]"
              label="Por página"
              density="compact"
              hide-details
              style="max-width: 140px"
            />

            <v-pagination
              v-model="page"
              :length="totalPages"
              :total-visible="5"
            />
          </div>
        </div>
      </v-card-text>
    </v-card>

    <ConfirmDeleteDialog
      v-model="deleteDialog"
      :contact="selectContact"
      :loading="deleting"
      :error="error"
      @confirm="confirmDelete"
      @cancel="closeDeleteDialog"
    />

    <ContactFormDialog
      v-model="formDialog"
      :mode="formMode"
      :initial-values="formInitialValues"
      :loading="saving"
      :error="formError"
      @save="handleSave"
    />
  </v-container>
</template>
