import { RegisterRequest } from '@core/models/register-request';
import { createAction, props } from '@ngrx/store';
import {
  User,
  LoginRequest,
  AuthCodeRequest,
  ResetPasswordRequest,
  AuthErrorKey,
} from './auth.models';
import { HttpErrorResponse } from '@angular/common/http';
import { ClientResponse } from '@core/models/client-response';

// Login

export const login = createAction(
  '[Auth] Try Login',
  props<{ request: LoginRequest }>()
);

export const loginSuccess = createAction(
  '[Auth] Login Succeded',
  props<{ userInfo: User }>()
);

export const loginFail = createAction(
  '[Auth] Login Failed',
  props<{ error: HttpErrorResponse }>()
);

// Logout

export const logout = createAction('[Auth] Logout');

export const logoutSuccess = createAction('[Auth] Logout Success');

// Register

export const register = createAction(
  '[Auth] Register',
  props<{ request: RegisterRequest }>()
);

export const registerSuccess = createAction(
  '[Auth] Register Succeded',
  props<{ userInfo: User }>()
);

export const registerFail = createAction(
  '[Auth] Register Failed',
  props<{ error: HttpErrorResponse }>()
);

// Confirm email

export const confirmEmail = createAction(
  '[Auth] Confirm Email',
  props<{ request: AuthCodeRequest }>()
);

export const confirmEmailSuccess = createAction(
  '[Auth] Confirm Email Succeded',
  props<{ userInfo: User }>()
);

export const confirmEmailFail = createAction(
  '[Auth] Confirm Email Failed',
  props<{ error: HttpErrorResponse }>()
);

// Request Password Reset

export const requestPasswordReset = createAction(
  '[Auth] Request Password Reset',
  props<{ email: string }>()
);

export const requestPasswordResetSuccess = createAction(
  '[Auth] Request Password Reset Succeded',
  props<{ response: ClientResponse }>()
);

export const requestPasswordResetFail = createAction(
  '[Auth] Request Password Reset Failed',
  props<{ error: HttpErrorResponse }>()
);

// Reset Password

export const resetPassword = createAction(
  '[Auth] Reset Password',
  props<{ request: ResetPasswordRequest }>()
);

export const resetPasswordSuccess = createAction(
  '[Auth] Reset Password Succeded',
  props<{ userInfo: User }>()
);

export const resetPasswordFail = createAction(
  '[Auth] Reset Password Failed',
  props<{ error: HttpErrorResponse }>()
);

// Clear Error

export const clearError = createAction(
  '[Auth] Clear Error',
  props<{ error: AuthErrorKey }>()
);

// Clear User Info

export const clearUserInfo = createAction('[Auth] Clear User Info');
