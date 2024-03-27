import { SMChannelDto } from '@lib/apiDefs';
import type { RootState } from '@lib/redux/store';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { useCallback } from 'react';
import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';

interface SelectedSMChannelPayload {
  value: SMChannelDto | undefined;
  typename: string;
}

type CurrentSelectedSMChannelState = Record<string, SMChannelDto | undefined>;

const initialState: CurrentSelectedSMChannelState = {};

const selectedSMChannelSlice = createSlice({
  name: 'selectedCurrentSelectedSMChannel',
  initialState,
  reducers: {
    setSMChannel: (state, action: PayloadAction<SelectedSMChannelPayload>) => {
      const { typename, value } = action.payload;
      state[typename] = value;
    },
    clearSMChannel: (state, action: PayloadAction<{ typename: string }>) => {
      delete state[action.payload.typename];
    }
  }
});

// Selectors
const selectedSelectedSMChannel = (typename: string) => (state: RootState) => state.SMChannelReducer[typename];

// Typed useSelector hook
const useTypedSelector: TypedUseSelectorHook<RootState> = useSelector;

// Hook
export const useSelectedSMChannel = (typename: string) => {
  const dispatch = useDispatch();
  const selectedSMChannel = useTypedSelector(selectedSelectedSMChannel(typename));

  const handleSetcurrentSelectedSMChannelSlice = useCallback(
    (newValue: SMChannelDto | undefined) => {
      dispatch(selectedSMChannelSlice.actions.setSMChannel({ typename, value: newValue }));
    },
    [dispatch, typename]
  );

  return { selectedSMChannel, setSelectedSMChannel: handleSetcurrentSelectedSMChannelSlice };
};

// export const { setSMChannel, clearSMChannel } = selectedSMChannelSlice.actions;

export default selectedSMChannelSlice.reducer;
