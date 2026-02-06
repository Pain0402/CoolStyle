<script setup lang="ts">
import MainLayout from '../layouts/MainLayout.vue';
import { useAuthStore } from '../stores/auth';
import { Package, User, Heart, Settings, LogOut } from 'lucide-vue-next';

const authStore = useAuthStore();
</script>

<template>
  <MainLayout>
    <div class="min-h-screen bg-[#050505] text-white pt-10 pb-20">
         <div class="container mx-auto px-6 grid grid-cols-1 lg:grid-cols-4 gap-8">
             
             <!-- Sidebar -->
             <div class="lg:col-span-1 space-y-6">
                 <!-- User Card -->
                 <div class="glass p-6 rounded-2xl flex items-center gap-4">
                     <div class="w-16 h-16 rounded-full bg-gradient-to-br from-cyan-500 to-purple-600 flex items-center justify-center text-2xl font-bold">
                         {{ authStore.user?.fullName?.charAt(0).toUpperCase() }}
                     </div>
                     <div>
                         <h3 class="font-bold font-display text-lg">{{ authStore.user?.fullName }}</h3>
                         <p class="text-xs text-gray-400">Member</p>
                     </div>
                 </div>

                 <!-- Menu -->
                 <div class="glass rounded-2xl overflow-hidden p-2">
                     <nav class="space-y-1">
                         <a href="#" class="flex items-center gap-3 px-4 py-3 bg-white/10 rounded-xl text-cyan-400 font-bold">
                             <User :size="18"/> My Profile
                         </a>
                         <a href="#" class="flex items-center gap-3 px-4 py-3 hover:bg-white/5 rounded-xl text-gray-400 hover:text-white transition">
                             <Package :size="18"/> Orders
                         </a>
                         <a href="#" class="flex items-center gap-3 px-4 py-3 hover:bg-white/5 rounded-xl text-gray-400 hover:text-white transition">
                             <Heart :size="18"/> Wishlist
                         </a>
                         <a href="#" class="flex items-center gap-3 px-4 py-3 hover:bg-white/5 rounded-xl text-gray-400 hover:text-white transition">
                             <Settings :size="18"/> Settings
                         </a>
                         <div class="h-px bg-white/10 my-2"></div>
                         <button @click="authStore.logout()" class="w-full flex items-center gap-3 px-4 py-3 hover:bg-red-500/10 rounded-xl text-red-500 transition">
                             <LogOut :size="18"/> Logout
                         </button>
                     </nav>
                 </div>
             </div>

             <!-- Content -->
             <div class="lg:col-span-3 space-y-8">
                 <div class="bg-white/5 rounded-3xl p-8 border border-white/10">
                     <h2 class="text-2xl font-bold font-display mb-6">Recent Orders</h2>
                     
                     <div class="space-y-4">
                         <!-- Empty State -->
                         <div class="text-center py-12">
                             <div class="w-16 h-16 bg-white/5 rounded-full flex items-center justify-center mx-auto mb-4 text-gray-600">
                                 <Package :size="32"/>
                             </div>
                             <p class="text-gray-400">No orders yet.</p>
                             <button class="mt-4 btn-primary bg-cyan-500 text-black px-6 py-2 rounded-lg hover:bg-cyan-400">Start Shopping</button>
                         </div>
                     </div>
                 </div>
                 
                 <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                     <div class="glass p-6 rounded-3xl">
                         <h3 class="font-bold mb-4">Account Details</h3>
                         <div class="space-y-2 text-sm text-gray-400">
                             <p>{{ authStore.user?.fullName }}</p>
                             <p>{{ authStore.user?.email }}</p>
                             <p>********</p>
                         </div>
                     </div>
                     <div class="glass p-6 rounded-3xl">
                         <h3 class="font-bold mb-4">Default Address</h3>
                         <div class="space-y-2 text-sm text-gray-400">
                             <p>Chưa có địa chỉ nào được lưu.</p>
                             <button class="text-cyan-500 text-xs font-bold uppercase mt-2">Add Address</button>
                         </div>
                     </div>
                 </div>
             </div>
         </div>
    </div>
  </MainLayout>
</template>
