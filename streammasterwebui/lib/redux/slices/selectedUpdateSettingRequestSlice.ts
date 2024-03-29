import type { RootState } from '@lib/redux/store';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { useCallback } from 'react';
import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';

interface UpdateSettingRequestPayload {
  value: UpdateSettingRequest;
  typename: string;
}

type CurrentUpdateSettingRequestState = Record<string, UpdateSettingRequest | undefined>;

const initialState: CurrentUpdateSettingRequestState = {};

const selectedUpdateSettingRequestSlice = createSlice({
  name: 'selectedCurrentUpdateSettingRequest',
  initialState,
  reducers: {
    setSelectedUpdateSettingRequest: (state, action: PayloadAction<UpdateSettingRequestPayload>) => {
      const { typename, value } = action.payload;
      state[typename] = value;
    },
    clearSelectedCurrentUpdateSettingRequest: (state, action: PayloadAction<{ typename: string }>) => {
      delete state[action.payload.typename];
    }
  }
});

// Selectors
const selectedUpdateSettingRequest = (typename: string) => (state: RootState) => state.selectUpdateSettingRequest[typename];

// Typed useSelector hook
const useTypedSelector: TypedUseSelectorHook<RootState> = useSelector;

// Hook
export const useSelectUpdateSettingRequest = (typename: string) => {
  const dispatch = useDispatch();
  const selectUpdateSettingRequest = useTypedSelector(selectedUpdateSettingRequest(typename));

  const handleSetcurrentUpdateSettingRequestSlice = useCallback(
    (newValue: UpdateSettingRequest) => {
      dispatch(selectedUpdateSettingRequestSlice.actions.setSelectedUpdateSettingRequest({ typename, value: newValue }));
    },
    [dispatch, typename]
  );

  return { selectUpdateSettingRequest, setSelectedUpdateSettingRequest: handleSetcurrentUpdateSettingRequestSlice };
};

export const { setSelectedUpdateSettingRequest, clearSelectedCurrentUpdateSettingRequest } = selectedUpdateSettingRequestSlice.actions;

export default selectedUpdateSettingRequestSlice.reducer;
