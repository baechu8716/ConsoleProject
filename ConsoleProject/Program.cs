using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;
using static System.Formats.Asn1.AsnWriter;

namespace ConsoleProject
{
    internal class Program
    {
        struct Position // 위치 정보 구조체
        {
            public int x;
            public int y;
        }
        static void Main(string[] args)
        { 
            bool gameOver = false;
            int life = 3; 
            int score = 0;
            bool isBonusStarActive = false;

            Position playerPos; 
            playerPos.x = 0;
            playerPos.y = 0;

            Position starPos;
            starPos.x = new Random().Next(1, 9);
            starPos.y = new Random().Next(1, 3);

            Position bonusstarPos;
            bonusstarPos.x = new Random().Next(1, 9);
            bonusstarPos.y = 2;

            char[,] stage;

            Show(); //게임 시작 화면 출력

            Start(ref playerPos, out stage,ref starPos);
            while(gameOver == false)
            {
                Render(playerPos, stage,ref starPos,ref score, life,ref bonusstarPos, ref isBonusStarActive);
                // 별의 y위치가 stage배열의 열보다 크다면(플레이어가 별을 못먹고 지나친다면) life를 하나 감소시키고 별의 위치를 랜덤 생성한다.
                if (starPos.y >= stage.GetLength(0))
                {
                    life --;
                    stage[starPos.y - 1, starPos.x] = ' ';
                    starPos.x = new Random().Next(1, 9); 
                    starPos.y = new Random().Next(1, 3);
                }
                // 보너스 별의 y위치가 stage배열의 열보다 크다면(플레이어가 별을 못먹고 지나친다면) life를 하나 감소시키고 보너별의 위치를 랜덤 생성한다.
                // 보너스 별이 y축으로 2칸씩 이동하다보니 배열을 넘어가서 자꾸 오류가 나길래 배열의 크기를 늘려버림
                if (bonusstarPos.y >= stage.GetLength(0))
                {
                    life--;
                    stage[bonusstarPos.y - 1, bonusstarPos.x] = ' ';
                    bonusstarPos.x = new Random().Next(1, 9);
                    bonusstarPos.y = 2;
                }
                
                ConsoleKey key = Input();
                Update(ref playerPos, key,ref starPos, stage,ref bonusstarPos, isBonusStarActive);

                // 목숨이 0일 경우 반복문 종료
                if (life == 0)
                {
                    gameOver = true;
                }
            }
            End(score);
        }

        static void Show()
        {
            Console.WriteLine("---------------------");
            Console.WriteLine("Console Game ProJect");
            Console.WriteLine("---------------------");
            Console.WriteLine("아무 키나 눌러 시작하세요");
            Console.ReadKey(true);
            Console.Clear();
        }

        static void Start(ref Position playerPos, out char[,] stage, ref Position starPos)
        {
            // 게임 설정 커서 표시 false
            Console.CursorVisible = false;

            // 플레이어 위치 설정
            playerPos.x = 5;
            playerPos.y = 14;
            
            // 스테이지 맵 설정
            stage = new char[16, 11]
            {
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'},
                {'▨', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▨'}
            };

        }

        static void Render(Position playerPos, char[,] stage,ref Position starPos,ref int score, int life,ref Position bonusstarPos, ref bool isBonusStarActive)
        {
            Console.SetCursorPosition(0,0); //커서 위치 0,0으로 console.clear보다 깔끔하게 출력됨
            Console.WriteLine($"라이프 : {life} 점수 : {score}");
            Console.ForegroundColor = ConsoleColor.Blue;
            PrintMap(stage); //맵출력
            Console.ResetColor();

            // 스코어 300점으로 나눠져 나머지가 0일 때마다 보너스 별출력을 위한 isBonusStarActive true로 변경
            if (score % 300 == 0 && score != 0 && !isBonusStarActive)
            {
                isBonusStarActive = true;
            }
            // 보너스 별이 비활성화 상태이고 별의 위치와 플레이어의 위치가 같을 때
            if (!isBonusStarActive && starPos.x == playerPos.x && playerPos.y == starPos.y)
            {
                // 별과 플레이어가 출력 안되고 ※ 출력 후 점수가 올라가고, 별 위치 초기화
                PrintScore(playerPos, ref starPos, ref score);
            }
            // 보너스 별이 활성화 상태이고 보너스별의 위치와 플레이어의 위치가 같을 때
            else if (isBonusStarActive && bonusstarPos.x == playerPos.x && bonusstarPos.y == playerPos.y)
            {
                // 보너스 별과 플레이어가 출력 안되고 ※ 출력 후 점수가 올라가고, 보너스 별 위치 초기화
                PrintBonusScore(playerPos,ref bonusstarPos,ref score,ref isBonusStarActive);
            }
            else
            {
                PrintPlayer(playerPos);
                if (isBonusStarActive) // 보너스별 활성화일때
                {
                    PrintBonusStar(bonusstarPos);
                }
                else // 비활성화라면 별 출력
                {
                    PrintStar(starPos);
                }
               
            }

        }
        
        // 맵 출력
        static void PrintMap(char[,] stage)
        {
            for (int y = 0; y < stage.GetLength(0); y++)
            {
                for (int x = 0; x < stage.GetLength(1); x++)
                {
                    Console.Write(stage[y, x]);
                }
                Console.WriteLine();
            }
        }

        // 플레이어 출력
        static void PrintPlayer(Position playerPos)
        {
            Console.SetCursorPosition(playerPos.x, playerPos.y);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("▼");
            Console.ResetColor();

        }

        // 별 출력
        static void PrintStar(Position starPos)
        {
                Console.SetCursorPosition(starPos.x, starPos.y);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("★");
                Console.ResetColor();
        }

        //보너스 별 출력
        static void PrintBonusStar(Position bonusstarPos)
        {
            Console.SetCursorPosition(bonusstarPos.x, bonusstarPos.y);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("☆");
            Console.ResetColor();
        }

        
        // 점수출력 후 별위치 초기화
        static void PrintScore(Position playerPos,ref Position starPos, ref int score)
        {
            Console.SetCursorPosition(playerPos.x, playerPos.y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("※\a");
            Console.ResetColor();
            starPos.x = new Random().Next(1, 9);
            starPos.y = new Random().Next(1, 3);
            score += 100;
        }
        
        // 보너스 점스 출력 후 보너스 별 위치 초기화 후 isBonusStarActive비활성화
        static void PrintBonusScore(Position playerPos, ref Position bonusstarPos, ref int score, ref bool isBonusStarActive)
        {
            Console.SetCursorPosition(playerPos.x, playerPos.y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("※\a");
            Console.ResetColor();
            bonusstarPos.x = new Random().Next(1, 9);
            bonusstarPos.y = 2;
            score += 500;
            isBonusStarActive = false;
        }

        // 사용자 입력을 받고 return값으로 출력
        static ConsoleKey Input()
        {
            return Console.ReadKey(true).Key;
        }

        static void Update(ref Position playerPos, ConsoleKey key,ref Position starPos, char[,] stage,ref Position bonusstarPos, bool isBonusStarActive)
        {
            Move(ref playerPos, key, ref starPos, stage,ref bonusstarPos, isBonusStarActive);

        }

        // 플레이어 이동
        static void Move(ref Position playerPos, ConsoleKey key, ref Position starPos, char[,] stage,ref Position bonusstarPos, bool isBonusStarActive)
        {
            Position targetPos;
            // 가로로만 이동. 벽을 만났을때는 이동하지 못함
            
                switch (key)
                {
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        targetPos.x = playerPos.x - 1;
                        targetPos.y = playerPos.y;
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        targetPos.x = playerPos.x + 1;
                        targetPos.y = playerPos.y;
                        break;
                case ConsoleKey.M: // 테스트를 위한 히든키..
                    targetPos.x = playerPos.x;
                    targetPos.y = playerPos.y;
                    break;
                default:
                        return;
                }
            // 움직일 위치가 빈칸일 때 이동하고 보너스별이 비활성화 상태라면 별을 y로 1칸, 활성화 상태라면 보너스 별을 y로 2칸 이동
            if (stage[targetPos.y, targetPos.x] == ' ')
            {
                playerPos = targetPos;
                if (!isBonusStarActive)
                { 
                    starPos.y++;
                }
                else
                {
                    bonusstarPos.y += 2;
                }
                //빈칸이 아닌 경우 못 움직인다. 벽에 막힌 경우
            }
            else
            {

            }
        }

        // 게임 종료
        static void End(int score)
        {
            Console.Clear();
            Console.WriteLine($"Game Over.. 당신의 점수는 {score}입니다.");
        }
    }
}
