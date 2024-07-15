import { GetEPGNextEPGNumber } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetEPGNextEPGNumber = createAsyncThunk('cache/getGetEPGNextEPGNumber', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetEPGNextEPGNumber');
    const response = await GetEPGNextEPGNumber();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});

