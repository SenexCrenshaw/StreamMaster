import { GetDownloadServiceStatus } from '@lib/smAPI/General/GeneralCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetDownloadServiceStatus = createAsyncThunk('cache/getGetDownloadServiceStatus', async (_: void, thunkAPI) => {
  try {
    const response = await GetDownloadServiceStatus();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


