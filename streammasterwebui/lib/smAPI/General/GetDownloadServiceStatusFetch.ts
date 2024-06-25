import { GetDownloadServiceStatus } from '@lib/smAPI/General/GeneralCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetDownloadServiceStatus = createAsyncThunk('cache/getGetDownloadServiceStatus', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetDownloadServiceStatus');
    const response = await GetDownloadServiceStatus();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


