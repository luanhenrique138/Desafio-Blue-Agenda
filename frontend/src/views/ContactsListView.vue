<script setup lang="ts">
import { onMounted, ref } from "vue";
import { ContactsService } from "@/services/contacts.service";
import type { Contact, CreateContactRequest } from "@/types/contacts";
import ContactsTable from "@/components/contacts/ContactsTable.vue"
import ConfirmDeleteDialog from "@/components/contacts/ConfirmDeleteDialog.vue"
import ContactFormDialog from "@/components/contacts/ContactFormDialog.vue"

const contacts = ref<Contact[]>([])

const deleteDialog = ref(false)
const formDialog = ref(false)
const formMode = ref<"create" | "edit">("create")
const formInitialValues = ref<Contact | null>(null)

const saving = ref(false)
const formError = ref<string | null>(null)

const error = ref<string | null>(null)
const deleting = ref(false)
const selectContact = ref<Contact | null>(null);

async function loadContacts() {
  try {
    error.value = null
    contacts.value = await ContactsService.getAllContacts()
  } catch (e) {
    error.value = "Falha ao carregar contatos."
  }
}

function openDeleteDialog(contact: Contact){
    selectContact.value = contact
    deleteDialog.value = true
}

function closeDeleteDialog(){
    deleteDialog.value = false
    selectContact.value = null
}

async function onDelete(contact: Contact) {
    await ContactsService.deleteContact(contact.id)
    loadContacts()
}

async function confirmDelete() {
    if(!selectContact.value) return;

    try {
        deleting.value = true
        await onDelete(selectContact.value)

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
      await ContactsService.createContact(payload) // precisa existir no service
    } else {
      // edit
      if (!formInitialValues.value) return
      await ContactsService.updateContact(formInitialValues.value.id, payload)
    }

    formDialog.value = false
    await loadContacts()
  } catch (e) {
    formError.value = "Falha ao salvar contato."
  } finally {
    saving.value = false
  }
}

onMounted(loadContacts)

</script>

<template>
  <v-container>
    <v-card>
      <v-card-title class="d-flex align-center justify-space-between">
            <v-card-title>Contatos</v-card-title>
            <v-btn
              color="primary"
              prepend-icon="mdi-plus"
              @click="openCreateDialog"
            >
              Adicionar
            </v-btn>
      </v-card-title>

      <v-card-text>
        <ContactsTable
          :contacts="contacts"
          @edit="openEditDialog"
          @delete="openDeleteDialog"
        />
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

<!-- <template>
  <v-container>
    <v-card>
      <v-card-title>
        Contatos
      </v-card-title>

      <v-card-text>
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
                        @click="onEdit(c)"
                    />
                    </template>
                </v-tooltip>

                <v-tooltip text="Excluir" location="top">
                    <template #activator="{ props }">
                    <v-btn
                        v-bind="props"
                        icon="mdi-delete"
                        variant="text"
                        @click="openDeleteDialog(c)"
                    />
                    </template>
                </v-tooltip>
                </td>
            </tr>
          </tbody>
        </v-table>
      </v-card-text>
    </v-card>
    
    <v-dialog v-model="deleteDialog" max-width="480">
        <v-card>
            <v-card-title>Confirmar exclusão</v-card-title>

            <v-card-text>
                Tem certeza que deseja excluir o contato
                <strong>{{ selectContact?.name }}</strong>?
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
                <v-btn variant="text" @click="closeDeleteDialog" :disabled="deleting">
                    Cancelar
                </v-btn>
                <v-btn color="red" @click="confirmDelete" :loading="deleting">
                    Excluir
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
    
  </v-container>
</template> -->


