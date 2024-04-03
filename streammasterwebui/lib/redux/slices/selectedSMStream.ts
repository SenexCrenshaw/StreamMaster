import type { RootState } from '@lib/redux/store';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { useCallback } from 'react';
import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';

interface SelectedSMStreamPayload {
  value: SMStreamDto | undefined;
  typename: string;
}

type CurrentSelectedSMStreamState = Record<string, SMStreamDto | undefined>;

const initialState: CurrentSelectedSMStreamState = {};

const selectedSMStreamSlice = createSlice({
  name: 'selectedCurrentSelectedSMStream',
  initialState,
  reducers: {
    setSMStream: (state, action: PayloadAction<SelectedSMStreamPayload>) => {
      const { typename, value } = action.payload;
      state[typename] = value;
    },
    clearSMStream: (state, action: PayloadAction<{ typename: string }>) => {
      delete state[action.payload.typename];
    }
  }
});

// Selectors
const selectedSelectedSMStream = (typename: string) => (state: RootState) => state.SMStreamReducer[typename];

// Typed useSelector hook
const useTypedSelector: TypedUseSelectorHook<RootState> = useSelector;

// Hook
export const useSelectedSMStream = (typename: string) => {
  const dispatch = useDispatch();
  const selectedSMStream = useTypedSelector(selectedSelectedSMStream(typename));

  const handleSetcurrentSelectedSMStreamSlice = useCallback(
    (newValue: SMStreamDto | undefined) => {
      dispatch(selectedSMStreamSlice.actions.setSMStream({ typename, value: newValue }));
    },
    [dispatch, typename]
  );

  return { selectedSMStream, setSelectedSMStream: handleSetcurrentSelectedSMStreamSlice };
};

// export const { setSMStream, clearSMStream } = selectedSMStreamSlice.actions;

export default selectedSMStreamSlice.reducer;
