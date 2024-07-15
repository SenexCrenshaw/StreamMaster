import { GetEPGColors } from '@lib/smAPI/EPG/EPGCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetEPGColors = createAsyncThunk('cache/getGetEPGColors', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetEPGColors');
    const response = await GetEPGColors();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});

