<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getOrders, updateOrderStatus } from '../services/order';
import apiClient from '../utils/api';
import DashboardChart from '../components/DashboardChart.vue';
import MainLayout from '../layouts/MainLayout.vue';
import { Package, DollarSign, ShoppingCart, TrendingUp } from 'lucide-vue-next';
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
const dashboardData = ref<any>(null);
const loading = ref(true);

const fetchDashboardStats = async () => {
    try {
        const stats: any = await apiClient.get('/admin/dashboard/revenue');
        dashboardData.value = stats;
        orders.value = stats.recentOrders || [];
    } catch(err) {
        console.error("Lỗi fetch dashboard:", err);
        // Fallback or handle later
    }
}

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
    'Pending': { label: 'Chờ xử lý', color: 'bg-yellow-500/10 text-yellow-400 border border-yellow-500/20', value: 0 },
    'Confirmed': { label: 'Đã xác nhận', color: 'bg-blue-500/10 text-cyan-400 border border-blue-500/20', value: 1 },
    'Shipped': { label: 'Đang giao', color: 'bg-indigo-500/10 text-indigo-400 border border-indigo-500/20', value: 2 },
    'Delivered': { label: 'Đã giao', color: 'bg-green-500/10 text-green-400 border border-green-500/20', value: 3 },
    'Cancelled': { label: 'Đã hủy', color: 'bg-red-500/10 text-red-500 border border-red-500/20', value: 4 },
};

onMounted(async () => {
    await fetchDashboardStats();
    loading.value = false;
});
</script>

<template>
  <MainLayout>
  <div class="min-h-screen bg-[#050505] text-white flex pt-20">
      <!-- Sidebar -->
      <aside class="w-64 bg-white/5 border-r border-white/10 hidden md:block backdrop-blur-md">
          <div class="h-16 flex items-center px-6 border-b border-white/10">
              <span class="font-display font-bold text-xl tracking-tight text-cyan-400 drop-shadow-[0_0_8px_rgba(0,242,234,0.5)]">ADMIN PANEL</span>
          </div>
          <nav class="p-4 space-y-2 mt-4">
              <a href="#" class="flex items-center gap-3 px-4 py-3 bg-cyan-400/10 text-cyan-400 font-bold rounded-xl border border-cyan-400/20 shadow-[0_0_10px_rgba(0,242,234,0.1)]">
                  <Package :size="20" /> Đơn hàng
              </a>
              <a href="#" class="flex items-center gap-3 px-4 py-3 text-gray-400 hover:bg-white/5 hover:text-white transition rounded-xl">
                  <Package :size="20" /> Sản phẩm (Sắp ra mắt)
              </a>
          </nav>
      </aside>

      <!-- Main Content -->
      <main class="flex-1 p-8 overflow-y-auto w-full">
          <div class="flex justify-between items-center mb-8">
              <h1 class="font-display text-3xl font-bold">Tổng quan Dashboard</h1>
              <div class="bg-white/5 px-4 py-2 rounded-xl border border-white/10 flex items-center gap-2 text-sm font-bold text-gray-300 backdrop-blur">
                  <TrendingUp :size="16" class="text-cyan-400" />
                  Báo cáo 7 Ngày
              </div>
          </div>

          <div v-if="loading" class="space-y-4">
              <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                  <div class="h-32 bg-white/5 rounded-2xl animate-pulse border border-white/10"></div>
                  <div class="h-32 bg-white/5 rounded-2xl animate-pulse border border-white/10"></div>
                  <div class="h-32 bg-white/5 rounded-2xl animate-pulse border border-white/10"></div>
              </div>
              <div v-for="i in 5" :key="i" class="h-16 bg-white/5 rounded-xl animate-pulse border border-white/10"></div>
          </div>

          <div v-else>
              <!-- Stats -->
              <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
                  <div class="bg-white/5 p-6 rounded-2xl border border-white/10 flex items-center gap-5 backdrop-blur-md relative overflow-hidden group hover:border-cyan-400/30 transition duration-300">
                      <div class="absolute -right-10 -top-10 w-32 h-32 bg-cyan-400/10 rounded-full blur-2xl group-hover:bg-cyan-400/20 transition duration-500"></div>
                      <div class="w-14 h-14 rounded-2xl bg-gradient-to-br from-cyan-400/20 to-blue-500/20 flex items-center justify-center text-cyan-400 border border-cyan-400/20">
                          <DollarSign :size="28" />
                      </div>
                      <div class="z-10">
                          <div class="text-sm font-bold text-gray-400 mb-1">Tổng doanh thu</div>
                          <div class="text-3xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-white to-gray-400">{{ formatCurrency(dashboardData?.totalRevenue || 0) }}</div>
                      </div>
                  </div>
                  <div class="bg-white/5 p-6 rounded-2xl border border-white/10 flex items-center gap-5 backdrop-blur-md relative overflow-hidden group hover:border-purple-400/30 transition duration-300">
                      <div class="absolute -right-10 -top-10 w-32 h-32 bg-purple-400/10 rounded-full blur-2xl group-hover:bg-purple-400/20 transition duration-500"></div>
                      <div class="w-14 h-14 rounded-2xl bg-gradient-to-br from-purple-400/20 to-pink-500/20 flex items-center justify-center text-purple-400 border border-purple-400/20">
                          <ShoppingCart :size="28" />
                      </div>
                      <div class="z-10">
                          <div class="text-sm font-bold text-gray-400 mb-1">Tổng đơn hàng</div>
                          <div class="text-3xl font-bold text-white">{{ dashboardData?.totalOrders || 0 }} Đơn</div>
                      </div>
                  </div>
                  <div class="bg-white/5 p-6 rounded-2xl border border-white/10 flex items-center gap-5 backdrop-blur-md relative overflow-hidden group hover:border-green-400/30 transition duration-300">
                      <div class="absolute -right-10 -top-10 w-32 h-32 bg-green-400/10 rounded-full blur-2xl group-hover:bg-green-400/20 transition duration-500"></div>
                      <div class="w-14 h-14 rounded-2xl bg-gradient-to-br from-green-400/20 to-emerald-500/20 flex items-center justify-center text-green-400 border border-green-400/20">
                           <TrendingUp :size="28" />
                      </div>
                      <div class="z-10">
                          <div class="text-sm font-bold text-gray-400 mb-1">Tỉ lệ tăng trưởng</div>
                          <div class="text-3xl font-bold text-green-400 drop-shadow-[0_0_5px_rgba(74,222,128,0.5)]">+12%</div>
                      </div>
                  </div>
              </div>

              <!-- Chart -->
              <div class="bg-white/5 p-8 rounded-3xl border border-white/10 mb-8 backdrop-blur" v-if="dashboardData?.chartData">
                  <h3 class="font-display font-bold text-xl mb-6 flex items-center gap-2">
                       <span class="w-2 h-6 bg-cyan-400 rounded-full block"></span> Biểu đồ doanh thu (7 ngày)
                  </h3>
                  <DashboardChart :chart-data="dashboardData.chartData" />
              </div>

              <h2 class="font-display text-2xl font-bold mb-6 flex items-center gap-2">
                  <span class="w-2 h-6 bg-purple-400 rounded-full block"></span> Đơn hàng gần đây
              </h2>
              <div class="bg-white/5 rounded-3xl border border-white/10 overflow-hidden backdrop-blur">
              <table class="w-full text-left">
                  <thead class="bg-black/40 border-b border-white/5 text-gray-400 text-xs uppercase font-bold tracking-wider">
                      <tr>
                          <th class="px-6 py-5">Mã đơn</th>
                          <th class="px-6 py-5">Khách hàng</th>
                          <th class="px-6 py-5">Tổng tiền</th>
                          <th class="px-6 py-5">Ngày đặt</th>
                          <th class="px-6 py-5">Trạng thái</th>
                          <th class="px-6 py-5">Hành động</th>
                      </tr>
                  </thead>
                  <tbody class="divide-y divide-white/5 text-sm">
                      <tr v-for="order in orders" :key="order.id" class="hover:bg-white/5 transition duration-200">
                          <td class="px-6 py-5 font-bold text-cyan-400">#{{ order.id }}</td>
                          <td class="px-6 py-5">
                              <div class="font-bold text-white">{{ order.customerName }}</div>
                              <div class="text-xs text-gray-400 mt-1">{{ order.customerEmail }}</div>
                          </td>
                          <td class="px-6 py-5 font-bold">{{ formatCurrency(order.totalAmount) }}</td>
                          <td class="px-6 py-5 text-gray-400">{{ formatDate(order.createdAt) }}</td>
                          <td class="px-6 py-5">
                              <span :class="['px-3 py-1 rounded-full text-xs font-bold inline-block', statusMap[order.status]?.color]">
                                  {{ statusMap[order.status]?.label || order.status }}
                              </span>
                          </td>
                          <td class="px-6 py-5">
                              <select 
                                :value="statusMap[order.status]?.value" 
                                @change="handleStatusChange(order.id, parseInt(($event.target as HTMLSelectElement).value))"
                                class="bg-black/60 text-white border border-white/20 text-sm rounded-lg px-3 py-2 outline-none focus:border-cyan-400 transition cursor-pointer hover:bg-white/5"
                              >
                                  <option class="bg-black" :value="0">Chờ xử lý</option>
                                  <option class="bg-black" :value="1">Xác nhận</option>
                                  <option class="bg-black" :value="2">Giao hàng</option>
                                  <option class="bg-black" :value="3">Hoàn tất</option>
                                  <option class="bg-black" :value="4">Hủy đơn</option>
                              </select>
                          </td>
                      </tr>
                  </tbody>
              </table>
              
              <div v-if="orders.length === 0" class="p-16 text-center text-gray-400">
                  <Package :size="48" class="mx-auto text-gray-600 mb-4" />
                  Chưa có đơn hàng nào cập bến.
              </div>
          </div>
          </div>
      </main>
  </div>
  </MainLayout>
</template>
