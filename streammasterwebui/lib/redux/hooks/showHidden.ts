import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetShowHiddenPayload {
  value: boolean | null;
  key: string;
}

type ShowHiddenState = Record<string, boolean | null>;

const initialState: ShowHiddenState = {};

const showHiddenSlice = createSlice({
  initialState,
  name: 'showHidden',
  reducers: {
    SetMultipleShowHidden: (state, action: PayloadAction<Record<string, boolean | null>>) => {
      Object.entries(action.payload).forEach(([key, value]) => {
        if (value === null) {
          state[key] = null;
        } else {
          state[key] = value;
        }
      });
    },
    SetShowHidden: (state, action: PayloadAction<SetShowHiddenPayload>) => {
      const { key, value } = action.payload;

      state[key] = value;
    }
  }
});

const selectShowHidden = createSelector(
  (state: RootState) => state.showHidden,
  (showHidden) => (key: string) => showHidden[key]
);

export const useShowHidden = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setShowHidden = (value: boolean | null) => {
    dispatch(
      showHiddenSlice.actions.SetShowHidden({
        key: key,
        value
      })
    );
  };

  if (key !== undefined && key !== '') {
    const persistKey = 'persist:showHidden';
    const persistData = localStorage.getItem(persistKey);

    if (persistData === null) {
      setShowHidden(null);
    } else {
      try {
        const parsedData = JSON.parse(persistData);
        if (!parsedData.hasOwnProperty(key)) {
          setShowHidden(null);
        }
      } catch (error) {}
    }
  }

  const setMultipleShowHidden = (values: Record<string, boolean | null>) => {
    dispatch(showHiddenSlice.actions.SetMultipleShowHidden(values));
  };

  const showHidden = useSelector((state: RootState) => selectShowHidden(state)(key));

  return { setMultipleShowHidden, setShowHidden, showHidden };
};

export default showHiddenSlice.reducer;
