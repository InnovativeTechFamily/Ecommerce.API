 //export const server = "https://e-shop-xi-vert.vercel.app/api/v2";
// export const backend_url = "https://e-shop-phi-plum.vercel.app/";
// export const soketEndPoint = "https://socket-server-e-shop-bso6.onrender.com/";

//  export const server = "http://localhost:8000/api/v2";
//  export const backend_url = "http://localhost:8000/";
 //export const soketEndPoint = "http://localhost:4000/";



export const server = process.env.REACT_APP_SERVER;
export const backend_url = process.env.REACT_APP_BACKEND_URL;
export const soketEndPoint = process.env.REACT_APP_SOCKET_ENDPOINT;
export const pageSize = process.env.REACT_APP_PAGE_SIZE;


 const dotnetserver = process.env.REACT_APP_DOT_NET_SERVER;
//user 
export const userCreate =dotnetserver +"/api/Users/create-user";
export const userLogin =dotnetserver+"/api/Users/login";
export const getUser =dotnetserver+"/api/Users/getUser";
export const activation =dotnetserver+"/api/Users/activation";
export const uploadAvatar =dotnetserver+"/api/Users/upload-avatar";
export const logout =dotnetserver+"/api/Users/logout";
//shop
export const shopCreateEndpoint =dotnetserver+"/api/Shop/create-shop";
export const shopActivationEndpoint =dotnetserver+"/api/Shop/activation";
export const shopLoginEndpoint =dotnetserver+"/api/Shop/login-shop";
export const shopDetailEnpoint =dotnetserver+"/api/Shop/getSeller";
export const shopInfoEnpoint =dotnetserver+"/api/Shop/get-shop-info";
export const updateShopAvatarEnpoint =dotnetserver+"/api/Shop/update-shop-avatar";
export const updateShopInfoEnpoint =dotnetserver+"/api/Shop/update-seller-info";
export const shopLogout =dotnetserver+"/api/Shop/logout";
export const getAllShopProducts =dotnetserver+"/api/Products/get-all-products-shop";
//Order

export const getAllOrdersEndpoint=dotnetserver+"/api/Orders/get-seller-all-orders";
export const createOrdersEndpoint=dotnetserver+"/api/Orders/create-order";
export const getAllOrdersForUserEndpoint=dotnetserver+"/api/Orders/get-all-orders";
export const getAdminAllOrdersEndpoint=dotnetserver+"/api/Orders/admin-all-orders";
export const updateOrderStatusEndpoint=dotnetserver+"/api/Orders/update-order-status/{id}";
export const updateOrderRefundEndpoint=dotnetserver+"/api/Orders/order-refund/{id}";
export const updateOrderRefundSuccessEndpoint=dotnetserver+"/api/Orders/order-refund-success/{id}";

//product
export const getAllProductsEndpoint=dotnetserver+"/api/Products/GetAllProduct";
export const updateProductEndpoint=dotnetserver+"/api/Products";

//events
export const getAllEventsEndpoint=dotnetserver+"/api/Events";






// const path = require("path");
// // config
// if (process.env.NODE_ENV !== "PRODUCTION") {
//     require("dotenv").config({
//       path: "config/.env",
//     });
//   }

// const server = process.env.SERVER;
// const backendUrl = process.env.BACKEND_URL;
// const socketEndPoint = process.env.SOCKET_ENDPOINT;
// console.log('Server:', server);
// console.log('Backend URL:', backend_url);
// console.log('Socket Endpoint:', soketEndPoint);