import type { RootState } from '@lib/redux/store';
import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { useCallback } from 'react';
import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';

interface CurrentSettingDtoPayload {
  value: SettingDto;
  typename: string;
}

type CurrentSettingDtoState = Record<string, SettingDto | undefined>;

const initialState: CurrentSettingDtoState = {};

const selectCurrentSettingDtoSlice = createSlice({
  name: 'selectedCurrentSettingDto',
  initialState,
  reducers: {
    setSelectedCurrentSettingDto: (state, action: PayloadAction<CurrentSettingDtoPayload>) => {
      const { typename, value } = action.payload;
      state[typename] = value;
    },
    clearSelectedCurrentSettingDto: (state, action: PayloadAction<{ typename: string }>) => {
      delete state[action.payload.typename];
    }
  }
});

// Selectors
const selectSelectedCurrentSettingDto = (typename: string) => (state: RootState) => state.selectCurrentSettingDto[typename];

// Typed useSelector hook
const useTypedSelector: TypedUseSelectorHook<RootState> = useSelector;

// Hook
export const useSelectCurrentSettingDto = (typename: string) => {
  const dispatch = useDispatch();
  const selectedCurrentSettingDto = useTypedSelector(selectSelectedCurrentSettingDto(typename));

  const handleSetcurrentSettingDtoSlice = useCallback(
    (newValue: SettingDto) => {
      dispatch(selectCurrentSettingDtoSlice.actions.setSelectedCurrentSettingDto({ typename, value: newValue }));
    },
    [dispatch, typename]
  );

  return { selectedCurrentSettingDto, setSelectedCurrentSettingDto: handleSetcurrentSettingDtoSlice };
};

export const { setSelectedCurrentSettingDto, clearSelectedCurrentSettingDto } = selectCurrentSettingDtoSlice.actions;

export default selectCurrentSettingDtoSlice.reducer;
