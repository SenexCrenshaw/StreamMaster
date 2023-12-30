import type { RootState } from '@lib/redux/store';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { useCallback } from 'react';
import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';

interface anySlicePayload {
  value: any;
  typename: string;
}

type CurrentanySliceState = Record<string, any | undefined>;

const initialState: CurrentanySliceState = {};

const selectedanySlice = createSlice({
  name: 'selectedCurrentanySlice',
  initialState,
  reducers: {
    setSelectedanySlice: (state, action: PayloadAction<anySlicePayload>) => {
      const { typename, value } = action.payload;
      state[typename] = value;
    },
    clearSelectedCurrentanySlice: (state, action: PayloadAction<{ typename: string }>) => {
      delete state[action.payload.typename];
    }
  }
});

// Selectors
const selectedany = (typename: string) => (state: RootState) => state.selectanySlice[typename];

// Typed useSelector hook
const useTypedSelector: TypedUseSelectorHook<RootState> = useSelector;

// Hook
export const useSelectanySlice = (typename: string) => {
  const dispatch = useDispatch();
  const selectanySlice = useTypedSelector(selectedany(typename));

  const handleSetcurrentanySlice = useCallback(
    (newValue: any) => {
      dispatch(selectedanySlice.actions.setSelectedanySlice({ typename, value: newValue }));
    },
    [dispatch, typename]
  );

  return { selectanySlice, setSelectedanySlice: handleSetcurrentanySlice };
};

export const { setSelectedanySlice, clearSelectedCurrentanySlice } = selectedanySlice.actions;

export default selectedanySlice.reducer;
