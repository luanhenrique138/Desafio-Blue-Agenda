import { createRouter, createWebHistory } from 'vue-router';
import ContactsListView from "@/views/ContactsListView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: "/", name: "contacts", component: ContactsListView },
  ],
})

export default router
