import apiClient from '../utils/api';

export interface CheckoutRequest {
    customerName: string;
    customerEmail: string;
    customerPhone: string;
    shippingAddress: string;
    note: string;
    items: {
        productId: number;
        quantity: number;
    }[];
}

export type OrderStatus = 'Pending' | 'Confirmed' | 'Shipped' | 'Delivered' | 'Cancelled';

export const createOrder = async (order: CheckoutRequest): Promise<any> => {
    return await apiClient.post('/orders', order);
};

export const getOrders = async (): Promise<any[]> => {
    return await apiClient.get('/orders/admin') as any[];
};

export const updateOrderStatus = async (orderId: number, status: number): Promise<any> => {
    // Enum mapping: Pending=0, Confirmed=1, Shipped=2, Delivered=3, Cancelled=4
    return await apiClient.put(`/orders/admin/${orderId}/status`, { status });
};
