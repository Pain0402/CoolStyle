<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { updateOrderStatus } from '../services/order';
import apiClient from '../utils/api';
import DashboardChart from '../components/DashboardChart.vue';
import MainLayout from '../layouts/MainLayout.vue';
import { Package, DollarSign, ShoppingCart, TrendingUp, RefreshCw } from 'lucide-vue-next';
import { useToast } from 'vue-toastification';

interface Order {
    id: number;
    customerName: string;
    customerEmail: string;
    totalAmount: number;
    createdAt: string;
    status: string;
    paymentMethod: string;
    paymentStatus: string;
}

const toast = useToast();

// Stats section (separate from orders)
const dashboardData = ref<any>(null);
const statsLoading = ref(true);

// Orders section (full list with filter/pagination)
const allOrders = ref<Order[]>([]);
const ordersLoading = ref(true);
const statusFilter = ref('All');
const currentPage = ref(1);
const pageSize = 10;

const filteredOrders = computed(() => {
    if (statusFilter.value === 'All') return allOrders.value;
    return allOrders.value.filter(o => o.status === statusFilter.value);
});

const paginatedOrders = computed(() => {
    const start = (currentPage.value - 1) * pageSize;
    return filteredOrders.value.slice(start, start + pageSize);
});

const totalPages = computed(() => Math.ceil(filteredOrders.value.length / pageSize));

const fetchDashboardStats = async () => {
    statsLoading.value = true;
    try {
        const stats: any = await apiClient.get('/admin/dashboard/revenue');
        dashboardData.value = stats;
    } catch (err) {
        toast.error('Không thể tải thống kê dashboard.');
    } finally {
        statsLoading.value = false;
    }
};

const fetchAllOrders = async () => {
    ordersLoading.value = true;
    try {
        const data: any = await apiClient.get('/orders/admin');
        allOrders.value = data as Order[];
    } catch (error) {
        toast.error('Không thể tải danh sách đơn hàng.');
    } finally {
        ordersLoading.value = false;
    }
};

const handleStatusChange = async (orderId: number, newStatus: number) => {
    try {
        await updateOrderStatus(orderId, newStatus);
        toast.success('Cập nhật trạng thái thành công!');
        // Update local state without refetching
        const order = allOrders.value.find(o => o.id === orderId);
        if (order) {
            const statusNames = ['Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled'];
            order.status = statusNames[newStatus] ?? order.status;
        }
    } catch {
        toast.error('Cập nhật thất bại.');
    }
};

const formatCurrency = (value: number) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);

const formatDate = (dateString: string) =>
    new Date(dateString).toLocaleDateString('vi-VN', {
        day: '2-digit', month: '2-digit', year: 'numeric',
        hour: '2-digit', minute: '2-digit'
    });

const statusMap: Record<string, { label: string; color: string; value: number }> = {
    Pending:   { label: 'Chờ xử lý',   color: 'bg-yellow-500/10 text-yellow-400 border border-yellow-500/20', value: 0 },
    Confirmed: { label: 'Đã xác nhận', color: 'bg-blue-500/10 text-cyan-400 border border-blue-500/20',   value: 1 },
    Shipped:   { label: 'Đang giao',   color: 'bg-indigo-500/10 text-indigo-400 border border-indigo-500/20', value: 2 },
    Delivered: { label: 'Đã giao',     color: 'bg-green-500/10 text-green-400 border border-green-500/20',  value: 3 },
    Cancelled: { label: 'Đã hủy',      color: 'bg-red-500/10 text-red-500 border border-red-500/20',       value: 4 },
};

onMounted(() => {
    fetchDashboardStats();
    fetchAllOrders();
});
</script>

<template>
  <MainLayout>
  <div class="min-h-screen bg-[#050505] text-white flex pt-20">
      <!-- Sidebar -->
      <aside class="w-64 bg-white/5 border-r border-white/10 hidden md:block backdrop-blur-md">
          <div class="h-16 flex items-center px-6 border-b border-white/10">
              <span class="font-display font-bold text-xl tracking-tight text-cyan-400">ADMIN PANEL</span>
          </div>
          <nav class="p-4 space-y-2 mt-4">
              <a href="#" class="flex items-center gap-3 px-4 py-3 bg-cyan-400/10 text-cyan-400 font-bold rounded-xl border border-cyan-400/20">
                  <Package :size="20" /> Đơn hàng
              </a>
          </nav>
      </aside>

      <!-- Main Content -->
      <main class="flex-1 p-8 overflow-y-auto w-full">
          <div class="flex justify-between items-center mb-8">
              <h1 class="font-display text-3xl font-bold">Tổng quan Dashboard</h1>
              <div class="bg-white/5 px-4 py-2 rounded-xl border border-white/10 flex items-center gap-2 text-sm font-bold text-gray-300">
                  <TrendingUp :size="16" class="text-cyan-400" /> Báo cáo 7 Ngày
              </div>
          </div>

          <!-- Stats Section -->
          <div v-if="statsLoading" class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
              <div v-for="i in 3" :key="i" class="h-32 bg-white/5 rounded-2xl animate-pulse border border-white/10"></div>
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
              <div class="bg-white/5 p-6 rounded-2xl border border-white/10 flex items-center gap-5 backdrop-blur-md relative overflow-hidden group hover:border-cyan-400/30 transition">
                  <div class="absolute -right-10 -top-10 w-32 h-32 bg-cyan-400/10 rounded-full blur-2xl"></div>
                  <div class="w-14 h-14 rounded-2xl bg-gradient-to-br from-cyan-400/20 to-blue-500/20 flex items-center justify-center text-cyan-400 border border-cyan-400/20">
                      <DollarSign :size="28" />
                  </div>
                  <div class="z-10">
                      <div class="text-sm font-bold text-gray-400 mb-1">Tổng doanh thu</div>
                      <div class="text-2xl font-bold text-white">{{ formatCurrency(dashboardData?.totalRevenue || 0) }}</div>
                  </div>
              </div>
              <div class="bg-white/5 p-6 rounded-2xl border border-white/10 flex items-center gap-5 backdrop-blur-md relative overflow-hidden group hover:border-purple-400/30 transition">
                  <div class="absolute -right-10 -top-10 w-32 h-32 bg-purple-400/10 rounded-full blur-2xl"></div>
                  <div class="w-14 h-14 rounded-2xl bg-gradient-to-br from-purple-400/20 to-pink-500/20 flex items-center justify-center text-purple-400 border border-purple-400/20">
                      <ShoppingCart :size="28" />
                  </div>
                  <div class="z-10">
                      <div class="text-sm font-bold text-gray-400 mb-1">Tổng đơn hàng</div>
                      <div class="text-2xl font-bold text-white">{{ dashboardData?.totalOrders || 0 }} Đơn</div>
                  </div>
              </div>
              <div class="bg-white/5 p-6 rounded-2xl border border-white/10 flex items-center gap-5 backdrop-blur-md relative overflow-hidden group hover:border-green-400/30 transition">
                  <div class="absolute -right-10 -top-10 w-32 h-32 bg-green-400/10 rounded-full blur-2xl"></div>
                  <div class="w-14 h-14 rounded-2xl bg-gradient-to-br from-green-400/20 to-emerald-500/20 flex items-center justify-center text-green-400 border border-green-400/20">
                      <TrendingUp :size="28" />
                  </div>
                  <div class="z-10">
                      <div class="text-sm font-bold text-gray-400 mb-1">Tổng sản phẩm bán</div>
                      <div class="text-2xl font-bold text-green-400">{{ allOrders.reduce((s, o) => s, 0) || '—' }}</div>
                  </div>
              </div>
          </div>

          <!-- Chart -->
          <div v-if="dashboardData?.chartData" class="bg-white/5 p-8 rounded-3xl border border-white/10 mb-8 backdrop-blur">
              <h3 class="font-display font-bold text-xl mb-6 flex items-center gap-2">
                  <span class="w-2 h-6 bg-cyan-400 rounded-full block"></span> Biểu đồ doanh thu (7 ngày)
              </h3>
              <DashboardChart :chart-data="dashboardData.chartData" />
          </div>

          <!-- Orders Management Section -->
          <div class="flex flex-wrap items-center justify-between gap-4 mb-6">
              <h2 class="font-display text-2xl font-bold flex items-center gap-2">
                  <span class="w-2 h-6 bg-purple-400 rounded-full block"></span>
                  Quản lý đơn hàng
                  <span class="text-sm font-normal text-gray-400 ml-2">({{ filteredOrders.length }} đơn)</span>
              </h2>
              <div class="flex items-center gap-3">
                  <!-- Status Filter -->
                  <select v-model="statusFilter" @change="currentPage = 1"
                          class="bg-black/60 text-white border border-white/20 text-sm rounded-lg px-3 py-2 outline-none focus:border-cyan-400 transition">
                      <option value="All">Tất cả trạng thái</option>
                      <option value="Pending">Chờ xử lý</option>
                      <option value="Confirmed">Đã xác nhận</option>
                      <option value="Shipped">Đang giao</option>
                      <option value="Delivered">Đã giao</option>
                      <option value="Cancelled">Đã hủy</option>
                  </select>
                  <button @click="fetchAllOrders" class="p-2 bg-white/5 border border-white/10 rounded-lg hover:bg-white/10 transition text-gray-400 hover:text-white">
                      <RefreshCw :size="16" />
                  </button>
              </div>
          </div>

          <!-- Orders Table -->
          <div class="bg-white/5 rounded-3xl border border-white/10 overflow-hidden backdrop-blur">
              <div v-if="ordersLoading" class="p-8 space-y-3">
                  <div v-for="i in 5" :key="i" class="h-14 bg-white/5 rounded-xl animate-pulse"></div>
              </div>
              <table v-else class="w-full text-left">
                  <thead class="bg-black/40 border-b border-white/5 text-gray-400 text-xs uppercase font-bold tracking-wider">
                      <tr>
                          <th class="px-6 py-4">Mã đơn</th>
                          <th class="px-6 py-4">Khách hàng</th>
                          <th class="px-6 py-4">Tổng tiền</th>
                          <th class="px-6 py-4">Ngày đặt</th>
                          <th class="px-6 py-4">Trạng thái</th>
                          <th class="px-6 py-4">Cập nhật</th>
                      </tr>
                  </thead>
                  <tbody class="divide-y divide-white/5 text-sm">
                      <tr v-for="order in paginatedOrders" :key="order.id" class="hover:bg-white/5 transition">
                          <td class="px-6 py-4 font-bold text-cyan-400">#{{ order.id }}</td>
                          <td class="px-6 py-4">
                              <div class="font-bold text-white">{{ order.customerName }}</div>
                              <div class="text-xs text-gray-400">{{ order.customerEmail }}</div>
                          </td>
                          <td class="px-6 py-4 font-bold">{{ formatCurrency(order.totalAmount) }}</td>
                          <td class="px-6 py-4 text-gray-400 text-xs">{{ formatDate(order.createdAt) }}</td>
                          <td class="px-6 py-4">
                              <span :class="['px-3 py-1 rounded-full text-xs font-bold inline-block', statusMap[order.status]?.color]">
                                  {{ statusMap[order.status]?.label || order.status }}
                              </span>
                          </td>
                          <td class="px-6 py-4">
                              <select :value="statusMap[order.status]?.value"
                                      @change="handleStatusChange(order.id, parseInt(($event.target as HTMLSelectElement).value))"
                                      class="bg-black/60 text-white border border-white/20 text-sm rounded-lg px-3 py-2 outline-none focus:border-cyan-400 transition cursor-pointer">
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

              <div v-if="!ordersLoading && filteredOrders.length === 0" class="p-16 text-center text-gray-400">
                  <Package :size="48" class="mx-auto text-gray-600 mb-4" />
                  Không có đơn hàng nào.
              </div>

              <!-- Pagination -->
              <div v-if="totalPages > 1" class="flex items-center justify-between px-6 py-4 border-t border-white/10">
                  <span class="text-sm text-gray-400">
                      Trang {{ currentPage }} / {{ totalPages }}
                  </span>
                  <div class="flex gap-2">
                      <button @click="currentPage--" :disabled="currentPage === 1"
                              class="px-3 py-1 rounded-lg border border-white/10 text-sm text-gray-400 hover:border-cyan-500 hover:text-cyan-400 disabled:opacity-30 disabled:cursor-not-allowed transition">
                          ← Trước
                      </button>
                      <button @click="currentPage++" :disabled="currentPage === totalPages"
                              class="px-3 py-1 rounded-lg border border-white/10 text-sm text-gray-400 hover:border-cyan-500 hover:text-cyan-400 disabled:opacity-30 disabled:cursor-not-allowed transition">
                          Sau →
                      </button>
                  </div>
              </div>
          </div>
      </main>
  </div>
  </MainLayout>
</template>
