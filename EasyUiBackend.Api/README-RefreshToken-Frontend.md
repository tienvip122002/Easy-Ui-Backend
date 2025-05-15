# Hướng dẫn sử dụng Refresh Token với React Frontend

## Tổng quan

Refresh token giúp người dùng không bị đăng xuất khi access token hết hạn. Khi access token hết hạn, hệ thống sẽ tự động sử dụng refresh token để lấy một cặp access token và refresh token mới thay vì yêu cầu người dùng đăng nhập lại.

## Thay đổi Store Redux

1. Cập nhật `src/redux/auth/authSlice.js`:

```javascript
import { createSlice } from '@reduxjs/toolkit';

const initialState = {
  isAuthenticated: false,
  token: localStorage.getItem('token') || null,
  refreshToken: localStorage.getItem('refreshToken') || null,
  userInfo: null,
  loading: false,
  error: null,
  tokenExpiry: localStorage.getItem('tokenExpiry') ? new Date(localStorage.getItem('tokenExpiry')) : null
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    loginStart: (state) => {
      state.loading = true;
      state.error = null;
    },
    loginSuccess: (state, action) => {
      state.isAuthenticated = true;
      state.token = action.payload.token;
      state.refreshToken = action.payload.refreshToken;
      state.tokenExpiry = new Date(action.payload.expiration);
      state.loading = false;

      // Lưu vào localStorage
      localStorage.setItem('token', action.payload.token);
      localStorage.setItem('refreshToken', action.payload.refreshToken);
      localStorage.setItem('tokenExpiry', action.payload.expiration);
    },
    loginFailure: (state, action) => {
      state.loading = false;
      state.error = action.payload;
      state.isAuthenticated = false;
    },
    logout: (state) => {
      state.isAuthenticated = false;
      state.token = null;
      state.refreshToken = null;
      state.tokenExpiry = null;
      state.userInfo = null;

      // Xóa khỏi localStorage
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('tokenExpiry');
    },
    setUserInfo: (state, action) => {
      state.userInfo = action.payload;
    }
  },
});

export const {
  loginStart,
  loginSuccess,
  loginFailure,
  logout,
  setUserInfo
} = authSlice.actions;

export default authSlice.reducer;
```

## Tạo Axios Instance với Interceptors

Tạo file mới `src/services/authService.js`:

```javascript
import axios from 'axios';
import { store } from '../redux/store';
import { loginSuccess, logout } from '../redux/auth/authSlice';

// API URL
const API_URL = 'https://your-api-url.com/api';

// Tạo axios instance 
const axiosInstance = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor để thêm token vào header
axiosInstance.interceptors.request.use(
  (config) => {
    const token = store.getState().auth.token;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor để refresh token nếu token hết hạn
axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    // Nếu lỗi 401 (Unauthorized) và chưa thử refresh token
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      try {
        const refreshToken = store.getState().auth.refreshToken;
        
        // Gọi API refresh token
        const response = await axios.post(`${API_URL}/Auth/refresh-token`, {
          refreshToken: refreshToken
        });
        
        const { token, refreshToken: newRefreshToken, expiration } = response.data;
        
        // Cập nhật token mới vào store
        store.dispatch(loginSuccess({
          token,
          refreshToken: newRefreshToken,
          expiration
        }));
        
        // Thử lại request ban đầu với token mới
        originalRequest.headers.Authorization = `Bearer ${token}`;
        return axios(originalRequest);
      } catch (error) {
        // Nếu refresh token thất bại, đăng xuất
        store.dispatch(logout());
        return Promise.reject(error);
      }
    }
    
    return Promise.reject(error);
  }
);

// Các API gọi
const authService = {
  login: async (email, password) => {
    try {
      const response = await axios.post(`${API_URL}/Auth/login`, {
        email,
        password
      });
      return response.data;
    } catch (error) {
      throw error;
    }
  },
  
  register: async (userData) => {
    try {
      const response = await axios.post(`${API_URL}/Auth/register`, userData);
      return response.data;
    } catch (error) {
      throw error;
    }
  },
  
  googleLogin: async (googleToken) => {
    try {
      const response = await axios.post(`${API_URL}/Auth/google-login`, {
        googleToken
      });
      return response.data;
    } catch (error) {
      throw error;
    }
  },
  
  refreshToken: async (refreshToken) => {
    try {
      const response = await axios.post(`${API_URL}/Auth/refresh-token`, {
        refreshToken
      });
      return response.data;
    } catch (error) {
      throw error;
    }
  },
  
  revokeToken: async () => {
    try {
      await axiosInstance.post(`${API_URL}/Auth/revoke-token`);
      return true;
    } catch (error) {
      throw error;
    }
  },
  
  logout: async () => {
    try {
      await authService.revokeToken();
    } catch (error) {
      console.error('Error revoking token:', error);
    }
    
    store.dispatch(logout());
  },
  
  getProfile: async () => {
    try {
      const response = await axiosInstance.get(`${API_URL}/Auth/profile`);
      return response.data;
    } catch (error) {
      throw error;
    }
  }
};

export { axiosInstance, authService };
```

## Cập nhật Login.js Component

```jsx
import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { authService } from '../services/authService';
import { loginStart, loginSuccess, loginFailure } from '../redux/auth/authSlice';

const Login = () => {
  const dispatch = useDispatch();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    dispatch(loginStart());
    
    try {
      const result = await authService.login(email, password);
      dispatch(loginSuccess(result));
    } catch (error) {
      dispatch(loginFailure(error.response?.data?.message || 'Đăng nhập thất bại'));
    }
  };
  
  const handleGoogleLogin = async (googleToken) => {
    dispatch(loginStart());
    
    try {
      const result = await authService.googleLogin(googleToken);
      dispatch(loginSuccess(result));
    } catch (error) {
      dispatch(loginFailure(error.response?.data?.message || 'Đăng nhập Google thất bại'));
    }
  };
  
  return (
    // JSX của component Login
  );
};

export default Login;
```

## Sử dụng axiosInstance cho tất cả API calls

Thay vì dùng `axios` trực tiếp, sử dụng `axiosInstance` đã tạo cho tất cả các API call:

```javascript
import { axiosInstance } from '../services/authService';

// Ví dụ:
const fetchData = async () => {
  try {
    const response = await axiosInstance.get('/api-endpoint');
    return response.data;
  } catch (error) {
    console.error('Error fetching data:', error);
    throw error;
  }
};
```

## Cài đặt Auth Guard (Tùy chọn)

Tạo một Higher Order Component để bảo vệ các route yêu cầu đăng nhập:

```jsx
import React from 'react';
import { useSelector } from 'react-redux';
import { Navigate } from 'react-router-dom';

const AuthGuard = ({ children }) => {
  const { isAuthenticated } = useSelector((state) => state.auth);
  
  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }
  
  return children;
};

export default AuthGuard;
```

Sử dụng trong routes:

```jsx
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import AuthGuard from './components/AuthGuard';
import Dashboard from './pages/Dashboard';
import Login from './pages/Login';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route 
          path="/dashboard" 
          element={
            <AuthGuard>
              <Dashboard />
            </AuthGuard>
          } 
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
```

## Tóm tắt quy trình hoạt động

1. Khi đăng nhập hoặc đăng ký, lưu trữ access token, refresh token và thời gian hết hạn vào Redux store và localStorage
2. Mỗi request, tự động gắn access token vào header
3. Khi gặp lỗi 401 (token hết hạn):
   - Tự động gọi endpoint refresh token
   - Cập nhật token mới vào Redux store và localStorage
   - Thử lại request ban đầu
4. Nếu refresh token thất bại (hết hạn, bị thu hồi), tự động đăng xuất người dùng

## Bảo mật Refresh Token

1. Không lưu refresh token trong cookie có thể truy cập bằng JavaScript
2. Cân nhắc sử dụng secure, HTTP-only cookies trên server-side nếu có thể
3. Luôn sử dụng HTTPS
4. Cài đặt thời gian hết hạn hợp lý cho refresh token 