import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector, TypedUseSelectorHook } from 'react-redux';
import { useCallback } from 'react';
import type { RootState } from '@lib/redux/store';

interface SelectedCountryPayload {
  value: string;
  typename: string;
}

type SelectedCountryState = Record<string, string | undefined>;

const initialState: SelectedCountryState = { Country: 'USA' };

const selectedCountrySlice = createSlice({
  name: 'selectedCountry',
  initialState,
  reducers: {
    setSelectedCountry: (state, action: PayloadAction<SelectedCountryPayload>) => {
      const { typename, value } = action.payload;
      state[typename] = value;
    },
    clearSelectedCountry: (state, action: PayloadAction<{ typename: string }>) => {
      delete state[action.payload.typename];
    }
  }
});

// Selectors
const selectSelectedCountry = (typename: string) => (state: RootState) => state.selectedCountry[typename];

// Typed useSelector hook
const useTypedSelector: TypedUseSelectorHook<RootState> = useSelector;

// Hook
export const useSelectedCountry = (typename: string) => {
  const dispatch = useDispatch();
  const selectedCountry = useTypedSelector(selectSelectedCountry(typename));

  const handleSetselectedCountrySlice = useCallback(
    (newValue: string) => {
      dispatch(selectedCountrySlice.actions.setSelectedCountry({ typename: typename, value: newValue }));
    },
    [dispatch, typename]
  );

  return { selectedCountry, setSelectedCountry: handleSetselectedCountrySlice };
};

export const { setSelectedCountry, clearSelectedCountry } = selectedCountrySlice.actions;

export default selectedCountrySlice.reducer;
