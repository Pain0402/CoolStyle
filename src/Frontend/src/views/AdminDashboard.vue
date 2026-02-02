<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getOrders, updateOrderStatus } from '../services/order';
import { Package } from 'lucide-vue-next';
import { useToast } from 'vue-toastification';

interface Order {
    id: number;
    customerName: string;
    customerEmail: string;
    totalAmount: number;
    createdAt: string;
    status: string;
}

const toast = useToast();
const orders = ref<Order[]>([]);
const loading = ref(true);

const fetchOrders = async () => {
    loading.value = true;
    try {
        const data = await getOrders();
        orders.value = data;
    } catch (error) {
        console.error(error);
        toast.error("Không thể tải danh sách đơn hàng.");
    } finally {
        loading.value = false;
    }
};

const handleStatusChange = async (orderId: number, newStatus: number) => {
    try {
        await updateOrderStatus(orderId, newStatus);
        toast.success("Cập nhật trạng thái thành công!");
        fetchOrders(); // Refresh
    } catch (error) {
        toast.error("Cập nhật thất bại.");
    }
};

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('vi-VN', {
        day: '2-digit', month: '2-digit', year: 'numeric',
        hour: '2-digit', minute: '2-digit'
    });
};

const statusMap: Record<string, { label: string, color: string, value: number }> = {
    'Pending': { label: 'Chờ xử lý', color: 'bg-yellow-100 text-yellow-800', value: 0 },
    'Confirmed': { label: 'Đã xác nhận', color: 'bg-blue-100 text-blue-800', value: 1 },
    'Shipped': { label: 'Đang giao', color: 'bg-indigo-100 text-indigo-800', value: 2 },
    'Delivered': { label: 'Đã giao', color: 'bg-green-100 text-green-800', value: 3 },
    'Cancelled': { label: 'Đã hủy', color: 'bg-red-100 text-red-800', value: 4 },
};

onMounted(() => {
    fetchOrders();
});
</script>

<template>
  <div class="min-h-screen bg-gray-50 flex">
      <!-- Sidebar -->
      <aside class="w-64 bg-white border-r border-gray-200 hidden md:block">
          <div class="h-16 flex items-center px-6 border-b border-gray-200">
              <span class="font-bold text-xl tracking-tight">COOLSTYLE ADMIN</span>
          </div>
          <nav class="p-4 space-y-1">
              <a href="#" class="flex items-center gap-3 px-4 py-3 bg-gray-100 text-black font-medium rounded-lg">
                  <Package :size="20" /> Đơn hàng
              </a>
              <a href="#" class="flex items-center gap-3 px-4 py-3 text-gray-500 hover:bg-gray-50 rounded-lg">
                  <Package :size="20" /> Sản phẩm (Sắp ra mắt)
              </a>
          </nav>
      </aside>

      <!-- Main Content -->
      <main class="flex-1 p-8 overflow-y-auto">
          <h1 class="text-2xl font-bold mb-6">Quản lý đơn hàng</h1>

          <div v-if="loading" class="space-y-4">
              <div v-for="i in 5" :key="i" class="h-16 bg-white rounded-lg shadow-sm animate-pulse"></div>
          </div>

          <div v-else class="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
              <table class="w-full text-left">
                  <thead class="bg-gray-50 border-b border-gray-100 text-gray-500 text-xs uppercase font-semibold">
                      <tr>
                          <th class="px-6 py-4">Mã đơn</th>
                          <th class="px-6 py-4">Khách hàng</th>
                          <th class="px-6 py-4">Tổng tiền</th>
                          <th class="px-6 py-4">Ngày đặt</th>
                          <th class="px-6 py-4">Trạng thái</th>
                          <th class="px-6 py-4">Hành động</th>
                      </tr>
                  </thead>
                  <tbody class="divide-y divide-gray-100">
                      <tr v-for="order in orders" :key="order.id" class="hover:bg-gray-50 transition">
                          <td class="px-6 py-4 font-medium">#{{ order.id }}</td>
                          <td class="px-6 py-4">
                              <div class="font-medium text-gray-900">{{ order.customerName }}</div>
                              <div class="text-xs text-gray-500">{{ order.customerEmail }}</div>
                          </td>
                          <td class="px-6 py-4 font-bold">{{ formatCurrency(order.totalAmount) }}</td>
                          <td class="px-6 py-4 text-sm text-gray-500">{{ formatDate(order.createdAt) }}</td>
                          <td class="px-6 py-4">
                              <span :class="['px-2.5 py-0.5 rounded-full text-xs font-medium', statusMap[order.status]?.color]">
                                  {{ statusMap[order.status]?.label || order.status }}
                              </span>
                          </td>
                          <td class="px-6 py-4">
                              <select 
                                :value="statusMap[order.status]?.value" 
                                @change="handleStatusChange(order.id, parseInt(($event.target as HTMLSelectElement).value))"
                                class="border-gray-300 text-sm rounded-md shadow-sm focus:border-black focus:ring-black"
                              >
                                  <option :value="0">Chờ xử lý</option>
                                  <option :value="1">Xác nhận</option>
                                  <option :value="2">Giao hàng</option>
                                  <option :value="3">Hoàn tất</option>
                                  <option :value="4">Hủy đơn</option>
                              </select>
                          </td>
                      </tr>
                  </tbody>
              </table>
              
              <div v-if="orders.length === 0" class="p-12 text-center text-gray-500">
                  Chưa có đơn hàng nào.
              </div>
          </div>
      </main>
  </div>
</template>
