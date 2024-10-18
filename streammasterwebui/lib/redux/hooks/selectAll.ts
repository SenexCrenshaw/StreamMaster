import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetSelectAllPayload {
  value: boolean;
  key: string;
}

type SelectAllState = Record<string, boolean>;

const initialState: SelectAllState = {};

const selectAllSlice = createSlice({
  initialState,
  name: 'selectAll',
  reducers: {
    SetSelectAll: (state, action: PayloadAction<SetSelectAllPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectSelectAll = createSelector(
  (state: RootState) => state.selectAll,
  (selectAll) => (key: string) => selectAll[key]
);

export const useSelectAll = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectAll = (value: boolean) => {
    dispatch(
      selectAllSlice.actions.SetSelectAll({
        key,
        value
      })
    );
  };

  const selectAll = useSelector((state: RootState) => selectSelectAll(state)(key));

  return { selectAll, setSelectAll };
};

export default selectAllSlice.reducer;
