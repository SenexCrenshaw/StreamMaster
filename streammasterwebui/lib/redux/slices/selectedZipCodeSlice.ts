import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector, TypedUseSelectorHook } from 'react-redux';
import { useCallback } from 'react';
import type { RootState } from '@lib/redux/store';

interface SelectedZipCodePayload {
  value: string;
  typename: string;
}

type SelectedZipCodeState = Record<string, string | undefined>;

const initialState: SelectedZipCodeState = {};

const selectedZipCodeSlice = createSlice({
  name: 'selectedZipCode',
  initialState,
  reducers: {
    setSelectedZipCode: (state, action: PayloadAction<SelectedZipCodePayload>) => {
      const { typename, value } = action.payload;
      state[typename] = value;
    },
    clearSelectedZipCode: (state, action: PayloadAction<{ typename: string }>) => {
      delete state[action.payload.typename];
    }
  }
});

// Selectors
const selectSelectedZipCode = (typename: string) => (state: RootState) => state.selectedCountry[typename];

// Typed useSelector hook
const useTypedSelector: TypedUseSelectorHook<RootState> = useSelector;

// Hook
export const useSelectedZipCode = (typename: string) => {
  const dispatch = useDispatch();
  const selectedZipCode = useTypedSelector(selectSelectedZipCode(typename));

  const handleSetselectedZipCodeSlice = useCallback(
    (newValue: string) => {
      dispatch(selectedZipCodeSlice.actions.setSelectedZipCode({ typename, value: newValue }));
    },
    [dispatch, typename]
  );

  return { selectedZipCode, setSelectedZipCode: handleSetselectedZipCodeSlice };
};

export const { setSelectedZipCode, clearSelectedZipCode } = selectedZipCodeSlice.actions;

export default selectedZipCodeSlice.reducer;
