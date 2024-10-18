import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetIsTruePayload {
  value: boolean;
  key: string;
}

type IsTrueState = Record<string, boolean>;

const initialState: IsTrueState = {};

const isTrueSlice = createSlice({
  initialState,
  name: 'isTrue',
  reducers: {
    SetIsTrue: (state, action: PayloadAction<SetIsTruePayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectIsTrue = createSelector(
  (state: RootState) => state.isTrue,
  (isTrue) => (key: string) => isTrue[key] ?? false
);

export const useIsTrue = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setIsTrue = (value: boolean) => {
    dispatch(
      isTrueSlice.actions.SetIsTrue({
        key,
        value
      })
    );
  };

  const isTrue = useSelector((state: RootState) => selectIsTrue(state)(key));

  return { isTrue, setIsTrue };
};

export default isTrueSlice.reducer;
