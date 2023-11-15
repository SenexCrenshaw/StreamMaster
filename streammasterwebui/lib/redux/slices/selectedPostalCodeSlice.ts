import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector, TypedUseSelectorHook } from 'react-redux';
import { useCallback } from 'react';
import type { RootState } from '@lib/redux/store';

interface SelectedPostalCodePayload {
  value: string;
  typename: string;
}

type SelectedPostalCodeState = Record<string, string | undefined>;

const initialState: SelectedPostalCodeState = {};

const selectedPostalCodeSlice = createSlice({
  name: 'selectedPostalCode',
  initialState,
  reducers: {
    setSelectedPostalCode: (state, action: PayloadAction<SelectedPostalCodePayload>) => {
      const { typename, value } = action.payload;
      state[typename] = value;
    },
    clearSelectedPostalCode: (state, action: PayloadAction<{ typename: string }>) => {
      delete state[action.payload.typename];
    }
  }
});

// Selectors
const selectSelectedPostalCode = (typename: string) => (state: RootState) => state.selectedPostalCode[typename];

// Typed useSelector hook
const useTypedSelector: TypedUseSelectorHook<RootState> = useSelector;

// Hook
export const useSelectedPostalCode = (typename: string) => {
  const dispatch = useDispatch();
  const selectedPostalCode = useTypedSelector(selectSelectedPostalCode(typename));

  const handleSetselectedPostalCodeSlice = useCallback(
    (newValue: string) => {
      dispatch(selectedPostalCodeSlice.actions.setSelectedPostalCode({ typename, value: newValue }));
    },
    [dispatch, typename]
  );

  return { selectedPostalCode, setSelectedPostalCode: handleSetselectedPostalCodeSlice };
};

export const { setSelectedPostalCode, clearSelectedPostalCode } = selectedPostalCodeSlice.actions;

export default selectedPostalCodeSlice.reducer;
