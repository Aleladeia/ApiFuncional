using ApiFuncional.Data;
using ApiFuncional.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; //muitas coisas da parte de WebAPI ainda tem um conexão de dependencia do MVC pois tudo
                                     // praticamente é filho de uma controller
namespace ApiFuncional.Controllers
{
    [ApiController] //Data Anotattion para dizer que se trata de uma controller de API
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        /*Injeção de dependência:
         * já vem embutida no Asp.Net, no contexto do EF mesmo nem precisamos declarar a injeção na 
         * Program.cs pois a própria definição que fizemos na Program.cs, informa o suficiente.
         */
        private readonly ApiDbContext _context; //criamos a propriedade privada _context, para receber a instânci de ApiDbContext 

        public ProdutosController(ApiDbContext context) //recebemos ApiDbContext no construtor
        {
            _context = context; //a propriedade privada _context está recebendo dados do contexto, assim criando a instância
        }
        //fim da injeção de dependência


        /* Criando os Métodos:
         * Primeiramente poderiamos tentar da forma abaixo, mas ela não vai ficar assim. 
         *
         * [HttpGet]
         * public IActionResult GetProdutos()
         * {
         * }
         * 
         * pois para retornarmos uma lista de produtos, vamos precisar do EF e com isso temos que fazer uma 
         * chamada Assincrona. Mudando a assinatura da nossa IActionResult, ficando da seguinte forma
        */
        [HttpGet]
        /*Sempre que trabalhamos com métodos assincronos o tipo de retorno definido vai ser sempre TASK na assinatura do método
         * e na nossa implementação não podemos implementar a Interface de Action Result mas sim a própria classe, que é a mesma
         * classe que implementa a interface. Depois passamos o que nossa ActionResult vai retornar, no caso uma coleção de 
         * Produtos, ficando então Task<ActionResult<IEnumerable<Produto>>>
        */
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos() 
        {                                       
            return await _context.Produtos.ToListAsync(); //retornando uma lista de produtos assincrona
            //aqui não foi utilizado o result ok, pois com ou sem ele o resultado retornado vai ser um 200
            //eles são mais usados quando queremos retornar algo mais diferenciado
        }

        [HttpGet("{id:int}")] //recebendo o id pela rota
        public async Task<ActionResult<Produto>> GetProduto(int id) //neste caso não é IEnumerable pois vai retornar apenas 1
        {
            var produto = await _context.Produtos.FindAsync(id); //FindAsync busca pela chave primaria da tabela

            return produto;  
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto) //Mantemos o produto pois no nosso post também
        {                                                                     //queremos que retorne o produto cadastrado
            _context.Produtos.Add(produto); //Adicionando o produto que recebemos via corpo no parametro do método
            await _context.SaveChangesAsync(); //Salvando o novo produto no banco de dados

            return CreatedAtAction(nameof(GetProduto),new {id = produto.Id}, produto); //retornando status 201
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            _context.Produtos.Update(produto); //Atualizando 1 produto
            await _context.SaveChangesAsync(); //Salvando alterações no banco de dados

            return NoContent();
        }

        [HttpDelete("{id:int}")] 
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            _context.Produtos.Remove(produto); //Removendo 1 produto
            await _context.SaveChangesAsync(); //Salvando alterações no banco de dados

            return NoContent();
        }
    }
}
