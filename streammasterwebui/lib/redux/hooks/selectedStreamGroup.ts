import { StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetSelectedStreamGroupPayload {
  value: StreamGroupDto | undefined;
  key: string;
}

type SelectedStreamGroupState = Record<string, StreamGroupDto | undefined>;

const initialState: SelectedStreamGroupState = {};

const selectedStreamGroupSlice = createSlice({
  initialState,
  name: 'selectedStreamGroup',
  reducers: {
    SetSelectedStreamGroup: (state, action: PayloadAction<SetSelectedStreamGroupPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectSelectedStreamGroup = createSelector(
  (state: RootState) => state.selectedStreamGroup,
  (selectedStreamGroup) => (key: string) => selectedStreamGroup[key]
);

export const useSelectedStreamGroup = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectedStreamGroup = (value: StreamGroupDto | undefined) => {
    dispatch(
      selectedStreamGroupSlice.actions.SetSelectedStreamGroup({
        key,
        value
      })
    );
  };

  const selectedStreamGroup = useSelector((state: RootState) => selectSelectedStreamGroup(state)(key));

  return { setSelectedStreamGroup, selectedStreamGroup };
};

export default selectedStreamGroupSlice.reducer;
