import { createSlice, Draft, PayloadAction } from '@reduxjs/toolkit';

interface AnyState {
  value: Record<string, any | undefined>;
}
const initialState: AnyState = {
  value: {}
};

const anySlice = createSlice({
  name: 'anyEntity',
  initialState,
  reducers: {
    setAnyValue: (state, action: PayloadAction<{ key: string; value: any | undefined }>) => {
      const { key, value } = action.payload;
      state.value[key] = value as Draft<any> | undefined;
    },
    removeAnyValue: (state, action: PayloadAction<string>) => {
      delete state.value[action.payload];
    }
  }
});
export const { setAnyValue, removeAnyValue } = anySlice.actions;
export default anySlice.reducer;
