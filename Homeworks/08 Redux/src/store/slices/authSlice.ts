import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';

export interface User {
  email: string;
  firstName: string;
  lastName: string;
}

interface AuthState {
  user: User | null;
  users: User[];
  isAuthenticated: boolean;
}

const initialState: AuthState = {
  user: null,
  users: [],
  isAuthenticated: false,
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    register: (state, action: PayloadAction<User>) => {
      state.users.push(action.payload);
      state.user = action.payload;
      state.isAuthenticated = true;
    },
    login: (state, action: PayloadAction<{ email: string }>) => {
      const found = state.users.find((u) => u.email === action.payload.email);
      if (!found) return;
      state.user = found;
      state.isAuthenticated = true;
    },
    logout: (state) => {
      state.user = null;
      state.isAuthenticated = false;
    },
  },
});

export const { register, login, logout } = authSlice.actions;
export default authSlice.reducer;
