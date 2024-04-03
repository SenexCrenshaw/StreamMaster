import { Draft, PayloadAction, createSlice } from '@reduxjs/toolkit';

interface QueryState<T> {
  value: Record<string, T | undefined>;
}

function createEntitySlice<T>() {
  const initialState: QueryState<T> = {
    value: {}
  };

  return createSlice({
    name: 'entity',
    initialState,
    reducers: {
      setValue: (state, action: PayloadAction<{ key: string; value: T | undefined }>) => {
        const { key, value } = action.payload;
        state.value[key] = value as Draft<T> | undefined;
      },
      removeValue: (state, action: PayloadAction<string>) => {
        delete state.value[action.payload];
      }
    }
  });
}
export default createEntitySlice;
