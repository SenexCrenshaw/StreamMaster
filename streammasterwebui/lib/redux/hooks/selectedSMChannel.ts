import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetSelectedSMChannelPayload {
  value: SMChannelDto | undefined;
  key: string;
}

type SelectedSMChannelState = Record<string, SMChannelDto | undefined>;

const initialState: SelectedSMChannelState = {};

const selectedSMChannelSlice = createSlice({
  initialState,
  name: 'selectedSMChannel',
  reducers: {
    SetSelectedSMChannel: (state, action: PayloadAction<SetSelectedSMChannelPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectSelectedSMChannel = createSelector(
  (state: RootState) => state.selectedSMChannel,
  (selectedSMChannel) => (key: string) => selectedSMChannel[key]
);

export const useSelectedSMChannel = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectedSMChannel = (value: SMChannelDto | undefined) => {
    dispatch(
      selectedSMChannelSlice.actions.SetSelectedSMChannel({
        key,
        value
      })
    );
  };

  const selectedSMChannel = useSelector((state: RootState) => selectSelectedSMChannel(state)(key));

  return { setSelectedSMChannel, selectedSMChannel };
};

export default selectedSMChannelSlice.reducer;
