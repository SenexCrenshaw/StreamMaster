import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';
import { UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

interface SetupdateSettingRequestPayload {
  value: UpdateSettingRequest;
  key: string;
}

type updateSettingRequestState = Record<string, UpdateSettingRequest>;

const initialState: updateSettingRequestState = {};

const updateSettingRequestSlice = createSlice({
  initialState,
  name: 'updateSettingRequest',
  reducers: {
    SetupdateSettingRequest: (state, action: PayloadAction<SetupdateSettingRequestPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectUpdateSettingRequest = createSelector(
  (state: RootState) => state.updateSettingRequest,
  (updateSettingRequest) => (key: string) => updateSettingRequest[key] ?? false
);

export const useUpdateSettingRequest = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setUpdateSettingRequest = (value: UpdateSettingRequest) => {
    dispatch(
      updateSettingRequestSlice.actions.SetupdateSettingRequest({
        key,
        value
      })
    );
  };

  const updateSettingRequest = useSelector((state: RootState) => selectUpdateSettingRequest(state)(key));

  return { setUpdateSettingRequest, updateSettingRequest };
};

export default updateSettingRequestSlice.reducer;
