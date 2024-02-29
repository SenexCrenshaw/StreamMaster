import { SmStreamDto } from '@lib/iptvApi';
import type { RootState } from '@lib/redux/store';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { useCallback } from 'react';
import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';

interface SMStreamsPayload {
  value: SmStreamDto;
  typename: string;
}

type CurrentSMStreamsState = Record<string, SmStreamDto | undefined>;

const initialState: CurrentSMStreamsState = {};

const selectedSMStreamsSlice = createSlice({
  name: 'selectedCurrentSMStreams',
  initialState,
  reducers: {
    setSelectedSMStreams: (state, action: PayloadAction<SMStreamsPayload>) => {
      const { typename, value } = action.payload;
      state[typename] = value;
    },
    clearSelectedCurrentSMStreams: (state, action: PayloadAction<{ typename: string }>) => {
      delete state[action.payload.typename];
    }
  }
});

// Selectors
const selectedSMStreams = (typename: string) => (state: RootState) => state.selectSMStreams[typename];

// Typed useSelector hook
const useTypedSelector: TypedUseSelectorHook<RootState> = useSelector;

// Hook
export const useSelectSMStreams = (typename: string) => {
  const dispatch = useDispatch();
  const selectSMStreams = useTypedSelector(selectedSMStreams(typename));

  const handleSetcurrentSMStreamsSlice = useCallback(
    (newValue: SmStreamDto) => {
      dispatch(selectedSMStreamsSlice.actions.setSelectedSMStreams({ typename, value: newValue }));
    },
    [dispatch, typename]
  );

  return { selectSMStreams, setSelectedSMStreams: handleSetcurrentSMStreamsSlice };
};

export const { setSelectedSMStreams, clearSelectedCurrentSMStreams } = selectedSMStreamsSlice.actions;

export default selectedSMStreamsSlice.reducer;
