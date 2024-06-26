import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetSelectedSMStreamsPayload {
  value: SMStreamDto[] | undefined;
  key: string;
}

type SelectedSMStreamsState = Record<string, SMStreamDto[] | undefined>;

const initialState: SelectedSMStreamsState = {};

const selectedSMStreamsSlice = createSlice({
  initialState,
  name: 'selectedSMStreams',
  reducers: {
    SetSelectedSMStreams: (state, action: PayloadAction<SetSelectedSMStreamsPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectSelectedSMStreams = createSelector(
  (state: RootState) => state.selectedSMStreams,
  (selectedSMStreams) => (key: string) => selectedSMStreams[key]
);

export const useSelectedSMStreams = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectedSMStreams = (value: SMStreamDto[] | undefined) => {
    dispatch(
      selectedSMStreamsSlice.actions.SetSelectedSMStreams({
        key,
        value
      })
    );
  };

  const selectedSMStreams = useSelector((state: RootState) => selectSelectedSMStreams(state)(key));

  return { setSelectedSMStreams, selectedSMStreams };
};

export default selectedSMStreamsSlice.reducer;
