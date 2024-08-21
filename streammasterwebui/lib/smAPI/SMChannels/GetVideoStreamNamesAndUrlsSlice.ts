import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';
import {FieldData, IdNameUrl } from '@lib/smAPI/smapiTypes';
import { fetchGetVideoStreamNamesAndUrls } from '@lib/smAPI/SMChannels/GetVideoStreamNamesAndUrlsFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: IdNameUrl[] | undefined;
  error: string | undefined;
  isError: boolean;
  isForced: boolean;
  isLoading: boolean;
}

const initialState: QueryState = {
  data: undefined,
  error: undefined,
  isError: false,
  isForced: false,
  isLoading: false
};

const getVideoStreamNamesAndUrlsSlice = createSlice({
  initialState,
  name: 'GetVideoStreamNamesAndUrls',
  reducers: {
    clear: (state) => {
      state = initialState;
      Logger.debug('GetVideoStreamNamesAndUrls clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      state.data = undefined;
      Logger.debug('GetVideoStreamNamesAndUrls clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      Logger.debug('GetVideoStreamNamesAndUrls setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      state.data = undefined;
      Logger.debug('GetVideoStreamNamesAndUrls  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      Logger.debug('GetVideoStreamNamesAndUrls setIsLoading ', action.payload.isLoading);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetVideoStreamNamesAndUrls.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetVideoStreamNamesAndUrls.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;
          setIsLoading({ isLoading: false });
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetVideoStreamNamesAndUrls.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        setIsLoading({ isLoading: false });
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getVideoStreamNamesAndUrlsSlice.actions;
export default getVideoStreamNamesAndUrlsSlice.reducer;
