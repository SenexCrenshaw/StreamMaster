import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';
import { SettingDto } from '@lib/smAPI/smapiTypes';

interface SetcurrentSettingRequestPayload {
  value: SettingDto;
  key: string;
}

type currentSettingRequestState = Record<string, SettingDto>;

const initialState: currentSettingRequestState = {};

const currentSettingRequestSlice = createSlice({
  initialState,
  name: 'currentSettingRequest',
  reducers: {
    SetcurrentSettingRequest: (state, action: PayloadAction<SetcurrentSettingRequestPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectCurrentSettingRequest = createSelector(
  (state: RootState) => state.currentSettingRequest,
  (currentSettingRequest) => (key: string) => currentSettingRequest[key] ?? false
);

export const useCurrentSettingRequest = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setCurrentSettingRequest = (value: SettingDto) => {
    dispatch(
      currentSettingRequestSlice.actions.SetcurrentSettingRequest({
        key,
        value
      })
    );
  };

  const currentSettingRequest = useSelector((state: RootState) => selectCurrentSettingRequest(state)(key));

  return { currentSettingRequest, setCurrentSettingRequest };
};

export default currentSettingRequestSlice.reducer;
