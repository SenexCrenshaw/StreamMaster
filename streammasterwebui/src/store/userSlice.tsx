import { createSlice } from '@reduxjs/toolkit'
import { type PayloadAction } from '@reduxjs/toolkit'
import { type RootState } from '../app/store'
import { type UserInformation } from "../common/common";

// Define a type for the slice state
type UserState = {
  userInformation: UserInformation
}

// Define the initial state using that type
const initialState: UserState = {
  userInformation: { IsAuthenticated: false, TokenAge: new Date() } as UserInformation,
}

export const userSlice = createSlice({
  initialState,
  name: 'user',
  reducers: {
    setUserInformation: (state, action: PayloadAction<UserInformation>) => {
      state.userInformation = action.payload
    },
  },
})

export const { setUserInformation } = userSlice.actions

// Other code such as selectors can use the imported `RootState` type
export const selectUserInformation = (state: RootState) => state.user.userInformation;

export default userSlice.reducer
