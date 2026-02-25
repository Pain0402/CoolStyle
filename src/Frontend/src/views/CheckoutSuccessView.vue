<script setup lang="ts">
import { useRoute } from 'vue-router';
import MainLayout from '../layouts/MainLayout.vue';
import { CheckCircle } from 'lucide-vue-next';

const route = useRoute();
const orderId = route.query.orderId || 'Không xác định';

</script>

<template>
  <MainLayout>
    <div class="min-h-[70vh] flex items-center justify-center bg-[#050505] text-white">
      <div class="max-w-md w-full p-8 bg-white/5 border border-white/10 rounded-3xl text-center backdrop-blur shadow-2xl">
        <div class="w-20 h-20 bg-green-500/20 text-green-400 rounded-full flex items-center justify-center mx-auto mb-6">
          <CheckCircle :size="40" />
        </div>
        
        <h1 class="font-display text-3xl font-bold mb-4">Thanh toán thành công!</h1>
        <p class="text-gray-400 mb-2">Cảm ơn bạn đã mua sắm tại CoolStyle.</p>
        <p class="text-gray-300 font-bold mb-8">Mã đơn hàng: #{{ orderId }}</p>

        <router-link to="/products" class="inline-block w-full bg-cyan-500 text-black py-4 rounded-xl font-bold hover:bg-cyan-400 transition mb-4 shadow-[0_0_15px_rgba(0,242,234,0.3)]">
          Tiếp tục mua sắm
        </router-link>
        
        <router-link to="/profile" class="inline-block w-full bg-white/10 border border-white/20 text-white py-4 rounded-xl font-bold hover:bg-white/20 transition">
          Xem đơn hàng của tôi
        </router-link>
      </div>
    </div>
  </MainLayout>
</template>
