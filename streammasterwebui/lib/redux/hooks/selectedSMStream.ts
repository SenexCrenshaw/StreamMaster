import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetSelectedSMStreamPayload {
  value: SMStreamDto | undefined;
  key: string;
}

type SelectedSMStreamState = Record<string, SMStreamDto | undefined>;

const initialState: SelectedSMStreamState = {};

const selectedSMStreamSlice = createSlice({
  initialState,
  name: 'selectedSMStream',
  reducers: {
    SetSelectedSMStream: (state, action: PayloadAction<SetSelectedSMStreamPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectSelectedSMStream = createSelector(
  (state: RootState) => state.selectedSMStream,
  (selectedSMStream) => (key: string) => selectedSMStream[key]
);

export const useSelectedSMStream = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectedSMStream = (value: SMStreamDto | undefined) => {
    dispatch(
      selectedSMStreamSlice.actions.SetSelectedSMStream({
        key,
        value
      })
    );
  };

  const selectedSMStream = useSelector((state: RootState) => selectSelectedSMStream(state)(key));

  return { setSelectedSMStream, selectedSMStream };
};

export default selectedSMStreamSlice.reducer;
